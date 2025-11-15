import { useState, useEffect } from 'react';
import {
  Modal,
  Button,
  Select,
  Group,
  Stack,
  NumberInput,
  Checkbox,
  LoadingOverlay,
} from '@mantine/core';
import { DatePickerInput } from '@mantine/dates';
import { useForm } from '@mantine/form';
import BankService from '../services/BankService';
import CurrencyService from '../services/CurrencyService';
import FundService from '../services/FundService';
import { notifications } from '@mantine/notifications';
import Logger from '../utils/Logger';
import SafeLogger from '@/utils/SafeLogger';
import type { CreateFundRequest } from '../services/types/FundTypes';
import type { Bank } from '../services/types/BankTypes';
import type { Currency } from '../services/types/CurrencyTypes';

interface CreateFundModalProps {
  opened: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

interface FormValues {
  bankId: string;
  currencyId: string;
  timeStamp: Date;
  amount: number;
  dailyUse: boolean;
}

export default function CreateFundModal({ opened, onClose, onSuccess }: CreateFundModalProps) {
  const [banks, setBanks] = useState<Bank[]>([]);
  const [currencies, setCurrencies] = useState<Currency[]>([]);
  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [loadingBanks, setLoadingBanks] = useState(false);
  const [loadingCurrencies, setLoadingCurrencies] = useState(false);
  const [initialValuesSet, setInitialValuesSet] = useState(false);

  Logger.info('Banks length:', banks.length);
  Logger.info('Currencies length:', currencies.length);

  // Define form with validation
  const form = useForm<FormValues>({
    initialValues: {
      bankId: '',
      currencyId: '',
      timeStamp: new Date(),
      amount: 0,
      dailyUse: true,
    },
    validate: {
      bankId: (value: string) => (!value ? 'Bank is required' : null),
      currencyId: (value: string) => (!value ? 'Currency is required' : null),
      amount: (value: number) => (value < 0 ? 'Amount must not be negative' : null),
    },
  });

  // Split data loading into separate effects to stagger the work
  // 1. First effect - load banks
  useEffect(() => {
    if (!opened) return;

    let mounted = true;
    setLoading(true);
    setLoadingBanks(true);

    Logger.info('Loading banks...');

    const loadBanks = async () => {
      try {
        Logger.info('Before BankService.getAllBanks() call');
        const banksData = await BankService.getAllBanks(true); // Force refresh
        Logger.info('Banks API response:', banksData);

        if (mounted) {
          // API now returns array directly
          const banksArray = Array.isArray(banksData) ? banksData : [];
          setBanks(banksArray);
          Logger.info('Banks state set to:', banksArray);

          // Auto-select the first bank only if no initial values have been set yet
          if (banksArray.length > 0 && !initialValuesSet) {
            form.setFieldValue('bankId', banksArray[0].id);
          }
        }
      } catch (error) {
        Logger.error('Failed to load banks:', error);
      } finally {
        if (mounted) setLoadingBanks(false);
      }
    };

    loadBanks();

    return () => {
      mounted = false;
    };
  }, [opened, initialValuesSet]);

  // 2. Second effect - load currencies
  useEffect(() => {
    if (!opened) return;

    let mounted = true;
    setLoadingCurrencies(true);

    Logger.info('Loading currencies...');

    const loadCurrencies = async () => {
      try {
        Logger.info('Before CurrencyService.getAllCurrencies() call');
        const currenciesData = await CurrencyService.getAllCurrencies(true); // Force refresh
        Logger.info('Currencies API response:', currenciesData);

        if (mounted) {
          // API now returns array directly
          const currenciesArray = Array.isArray(currenciesData) ? currenciesData : [];
          setCurrencies(currenciesArray);
          Logger.info('Currencies state set to:', currenciesArray);

          // Auto-select the first currency (which will be Peso due to sorting) only if no initial values have been set
          if (currenciesArray.length > 0 && !initialValuesSet) {
            // Find Peso first, otherwise use first available
            const pesoId = '6d189135-7040-45a1-b713-b1aa6cad1720';
            const pesoCurrency = currenciesArray.find(c => c.id === pesoId);
            const selectedCurrencyId = pesoCurrency ? pesoId : currenciesArray[0].id;
            form.setFieldValue('currencyId', selectedCurrencyId);
            setInitialValuesSet(true);
          }
        }
      } catch (error) {
        Logger.error('Failed to load currencies:', error);

        if (mounted) {
          notifications.show({
            title: 'Error',
            message: 'Failed to load currencies',
            color: 'red',
            autoClose: 5000,
          });
        }
      } finally {
        if (mounted) {
          setLoadingCurrencies(false);
          setLoading(false); // All loading complete
        }
      }
    };

    loadCurrencies();

    return () => {
      mounted = false;
    };
  }, [opened, initialValuesSet]);

  // Clean up when modal closes
  useEffect(() => {
    if (!opened) {
      let isMounted = true;

      const cleanup = () => {
        if (isMounted) {
          form.reset();
          setInitialValuesSet(false);
        }
      };

      if (typeof window.requestIdleCallback === 'function') {
        const idleId = requestIdleCallback(cleanup);
        return () => {
          isMounted = false;
          cancelIdleCallback(idleId);
        };
      } else {
        const timeoutId = setTimeout(cleanup, 50);
        return () => {
          isMounted = false;
          clearTimeout(timeoutId);
        };
      }
    }
  }, [opened]);

  // Handle form submission
  const handleSubmit = async (values: FormValues) => {
    setSubmitting(true);
    try {
      // Format the request
      const request: CreateFundRequest = {
        bankId: values.bankId,
        currencyId: values.currencyId,
        timeStamp: values.timeStamp.toISOString(),
        amount: values.amount,
        dailyUse: values.dailyUse,
      };

      // Send the request
      await FundService.createFund(request);

      setTimeout(() => {
        notifications.show({
          title: 'Success',
          message: 'Fund created successfully',
          color: 'green',
          autoClose: 3000,
        });

        form.reset();
        setSubmitting(false);
        onSuccess();
        onClose();
      }, 0);
    } catch (error) {
      SafeLogger.error('Failed to create fund:', error);

      setTimeout(() => {
        notifications.show({
          title: 'Error',
          message: 'Failed to create fund. Please try again.',
          color: 'red',
          autoClose: 5000,
        });
        setSubmitting(false);
      }, 0);
    }
  };

  // Prepare select data directly in render
  const bankSelectData = banks.map(bank => ({
    value: bank.id,
    label: bank.name,
  }));

  const currencySelectData = currencies
    .map(currency => ({
      value: currency.id,
      label: `${currency.name} (${currency.shortName})`,
    }))
    .sort((a, b) => {
      // Put Peso first, Dollar second by ID
      const PESO_CURRENCY_ID = '6d189135-7040-45a1-b713-b1aa6cad1720';
      const DOLLAR_CURRENCY_ID = 'efbf50bc-34d4-43e9-96f9-9f6213ea11b5';

      if (a.value === PESO_CURRENCY_ID) return -1;
      if (b.value === PESO_CURRENCY_ID) return 1;

      if (a.value === DOLLAR_CURRENCY_ID) return -1;
      if (b.value === DOLLAR_CURRENCY_ID) return 1;

      return a.label.localeCompare(b.label);
    });

  return (
    <Modal opened={opened} onClose={onClose} title="Update Funds" centered size="md">
      <LoadingOverlay visible={loading} />
      <form onSubmit={form.onSubmit(handleSubmit)}>
        <Stack>
          <Select
            data={bankSelectData}
            label="Bank"
            placeholder="Select a bank"
            required
            searchable
            withAsterisk
            disabled={loadingBanks}
            {...form.getInputProps('bankId')}
          />

          <Select
            data={currencySelectData}
            label="Currency"
            placeholder="Select a currency"
            required
            searchable
            withAsterisk
            disabled={loadingCurrencies}
            {...form.getInputProps('currencyId')}
          />

          <NumberInput
            label="Amount"
            placeholder="Enter amount"
            required
            min={0}
            step={0.01}
            {...form.getInputProps('amount')}
          />

          <DatePickerInput
            label="Date"
            placeholder="Select a date"
            required
            clearable={false}
            valueFormat="YYYY-MM-DD"
            {...form.getInputProps('timeStamp')}
          />

          <Checkbox label="Daily use" {...form.getInputProps('dailyUse', { type: 'checkbox' })} />

          <Group justify="flex-end" mt="md">
            <Button variant="outline" onClick={onClose} type="button">
              Cancel
            </Button>
            <Button
              type="submit"
              loading={submitting}
              color="teal"
              disabled={loading || submitting}
            >
              Save
            </Button>
          </Group>
        </Stack>
      </form>
    </Modal>
  );
}
