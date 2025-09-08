import { useState, useCallback } from 'react';
import { Modal, NumberInput, Select, Button, Stack, Group, Text, Alert } from '@mantine/core';
import { useForm } from '@mantine/form';
import { notifications } from '@mantine/notifications';
import { IconCurrencyDollar, IconInfoCircle } from '@tabler/icons-react';
import CurrencyExchangeRateService from '../services/CurrencyExchangeRateService';
import CurrencyService from '../services/CurrencyService';
import SafeLogger from '@/utils/SafeLogger';
import type { CreateCurrencyExchangeRateCommand } from '../services/types/CurrencyExchangeRateTypes';
import type { Currency } from '../services/types/CurrencyTypes';
import { useEffect } from 'react';
import { CURRENCY_IDS } from '../constants/currencies';

interface CreateExchangeRateModalProps {
  opened: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

interface FormValues {
  baseCurrencyId: string;
  quoteCurrencyId: string;
  buyRate: number | '';
  sellRate: number | '';
}

const CreateExchangeRateModal = ({ opened, onClose, onSuccess }: CreateExchangeRateModalProps) => {
  const [loading, setLoading] = useState(false);
  const [currencies, setCurrencies] = useState<Currency[]>([]);
  const [loadingCurrencies, setLoadingCurrencies] = useState(true);

  const form = useForm<FormValues>({
    initialValues: {
      baseCurrencyId: CURRENCY_IDS.ARS, // Default to ARS
      quoteCurrencyId: CURRENCY_IDS.USD, // Default to USD
      buyRate: 0.001, // Default buy rate (ARS to USD)
      sellRate: 0.00102, // Default sell rate (ARS to USD)
    },
    validate: {
      baseCurrencyId: value => (!value ? 'Base currency is required' : null),
      quoteCurrencyId: value => (!value ? 'Quote currency is required' : null),
      buyRate: value => {
        if (value === '' || value === null || value === undefined || value === 0)
          return 'Buy rate is required';
        if (typeof value === 'number' && value <= 0) return 'Buy rate must be greater than 0';
        return null;
      },
      sellRate: value => {
        if (value === '' || value === null || value === undefined || value === 0)
          return 'Sell rate is required';
        if (typeof value === 'number' && value <= 0) return 'Sell rate must be greater than 0';
        return null;
      },
    },
  });

  const fetchCurrencies = useCallback(async () => {
    setLoadingCurrencies(true);
    try {
      const response = await CurrencyService.getAllCurrencies();
      const activeCurrencies = Array.isArray(response)
        ? response.filter(currency => !currency.deactivated)
        : [];
      setCurrencies(activeCurrencies);
    } catch (error) {
      SafeLogger.error('Error fetching currencies:', error);
      notifications.show({
        title: 'Error',
        message: 'Failed to load currencies. Please try again.',
        color: 'red',
      });
    } finally {
      setLoadingCurrencies(false);
    }
  }, []);

  // Fetch currencies when modal opens
  useEffect(() => {
    if (opened) {
      fetchCurrencies();
    }
  }, [opened, fetchCurrencies]);

  const handleSubmit = useCallback(
    async (values: FormValues) => {
      if (values.baseCurrencyId === values.quoteCurrencyId) {
        form.setFieldError(
          'quoteCurrencyId',
          'Quote currency must be different from base currency'
        );
        return;
      }

      setLoading(true);
      try {
        const command: CreateCurrencyExchangeRateCommand = {
          baseCurrencyId: values.baseCurrencyId,
          quoteCurrencyId: values.quoteCurrencyId,
          buyRate: Number(values.buyRate),
          sellRate: Number(values.sellRate),
        };

        await CurrencyExchangeRateService.createExchangeRate(command);

        notifications.show({
          title: 'Success',
          message: 'Currency exchange rate created successfully!',
          color: 'green',
        });

        form.reset();
        onSuccess();
        onClose();
      } catch (error) {
        SafeLogger.error('Error creating exchange rate:', error);
        notifications.show({
          title: 'Error',
          message: 'Failed to create exchange rate. Please try again.',
          color: 'red',
        });
      } finally {
        setLoading(false);
      }
    },
    [form, onSuccess, onClose]
  );

  const handleClose = useCallback(() => {
    form.reset();
    onClose();
  }, [form, onClose]);

  // Prepare currency options for select components
  const currencyOptions = currencies.map(currency => ({
    value: currency.id,
    label: `${currency.name} (${currency.shortName})`,
  }));

  const selectedBaseCurrency = currencies.find(c => c.id === form.values.baseCurrencyId);
  const selectedQuoteCurrency = currencies.find(c => c.id === form.values.quoteCurrencyId);

  return (
    <Modal
      opened={opened}
      onClose={handleClose}
      title={
        <Group gap="xs">
          <IconCurrencyDollar size="1.2rem" />
          <Text fw={600}>Add New Exchange Rate</Text>
        </Group>
      }
      size="md"
      centered
    >
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Stack gap="md">
          <Alert
            icon={<IconInfoCircle size="1rem" />}
            title="Exchange Rate Information"
            color="blue"
            variant="light"
          >
            <Text size="sm">
              Create an exchange rate between two currencies. The buy rate is the rate at which you
              buy the quote currency, and the sell rate is the rate at which you sell it.
            </Text>
          </Alert>

          <Select
            label="Base Currency"
            placeholder="Select base currency"
            data={currencyOptions}
            searchable
            required
            disabled={loadingCurrencies}
            {...form.getInputProps('baseCurrencyId')}
          />

          <Select
            label="Quote Currency"
            placeholder="Select quote currency"
            data={currencyOptions}
            searchable
            required
            disabled={loadingCurrencies}
            {...form.getInputProps('quoteCurrencyId')}
          />

          {selectedBaseCurrency && selectedQuoteCurrency && (
            <Alert color="teal" variant="light">
              <Text size="sm">
                Exchange rate: {selectedBaseCurrency.shortName} → {selectedQuoteCurrency.shortName}
              </Text>
            </Alert>
          )}

          <NumberInput
            label="Buy Rate"
            placeholder="Enter buy rate"
            min={0}
            decimalScale={8}
            stepHoldDelay={500}
            stepHoldInterval={100}
            required
            {...form.getInputProps('buyRate')}
          />

          <NumberInput
            label="Sell Rate"
            placeholder="Enter sell rate"
            min={0}
            decimalScale={8}
            stepHoldDelay={500}
            stepHoldInterval={100}
            required
            {...form.getInputProps('sellRate')}
          />

          <Group justify="flex-end" gap="sm" mt="md">
            <Button variant="outline" onClick={handleClose} disabled={loading}>
              Cancel
            </Button>
            <Button type="submit" loading={loading} color="teal">
              Save
            </Button>
          </Group>
        </Stack>
      </form>
    </Modal>
  );
};

export default CreateExchangeRateModal;
