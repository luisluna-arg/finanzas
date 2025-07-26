import { useAuth } from '../auth';
import {
  Title,
  Text,
  Card,
  Group,
  Stack,
  ThemeIcon,
  Paper,
  Box,
  Table,
  Divider,
  Loader,
  Center,
  ScrollArea,
  Button,
} from '@mantine/core';
import { IconReceipt2, IconBuildingBank, IconPlus } from '@tabler/icons-react';
import { useState, useEffect, useCallback, useMemo } from 'react';
import FundService from '../services/FundService';
import type { Fund } from '../services/types/FundTypes';
import CreateFundModal from '../components/CreateFundModal';
import { getMaxDecimals } from '../constants/currencies';

const FundsDashboard = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [funds, setFunds] = useState<Fund[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [createModalOpen, setCreateModalOpen] = useState(false);

  // Memoized card configuration to avoid recreation
  const fundCard = useMemo(
    () => ({
      title: 'Funds',
      description: 'Track your available funds',
      icon: <IconReceipt2 size="1.5rem" stroke={1.5} />,
      color: 'orange',
    }),
    []
  );

  // Helper function to format number without currency symbol
  const formatNumber = useCallback((amount: number, currencyId?: string) => {
    // Use 2 decimals for USD and ARS (pesos), 8 for others
    const maxDecimals = getMaxDecimals(currencyId);

    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: maxDecimals,
    }).format(amount);
  }, []);

  // Helper function to extract bank/account name from fund label
  const extractBankFromLabel = useCallback((label: string): string => {
    // Extract the bank/account name from the label
    const parts = label.split(' ');
    // Return all parts except the last one (which is likely the currency)
    return parts.slice(0, -1).join(' ').trim();
  }, []);

  // Calculate total once
  const totalValue = useMemo(() => {
    const total = funds.reduce((sum, fund) => sum + fund.quoteCurrencyValue, 0);
    return Math.round(total * 100) / 100;
  }, [funds]);

  // Default currency and symbol - calculated once
  const defaultCurrencyInfo = useMemo(() => {
    if (funds.length === 0) return { currency: '', symbol: '', currencyId: '' };
    return {
      currency: funds[0].defaultCurrency,
      symbol: funds[0].defaultCurrencySymbol || '',
      currencyId: funds[0].defaultCurrencyId,
    };
  }, [funds]);

  // Effect for media query - passive resize listener
  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
    };

    // Use passive event listener for better performance
    window.addEventListener('resize', handleResize, { passive: true });
    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  // Fetch funds data
  const fetchFunds = useCallback(async (showLoading = true) => {
    if (showLoading) setLoading(true);

    try {
      // Request can be handled in another frame
      const response = await FundService.getAllFunds(true); // Force refresh to get latest data

      // Update state in the next frame to avoid blocking the main thread
      setTimeout(() => {
        setFunds(response.items || []);
        setError(null);
        if (showLoading) setLoading(false);
      }, 0);
    } catch (err) {
      console.error('Error fetching funds:', err);

      setTimeout(() => {
        setError('Failed to load funds data. Please try again later.');
        if (showLoading) setLoading(false);
      }, 0);
    }
  }, []);

  // Initial data fetch
  useEffect(() => {
    fetchFunds();
  }, [fetchFunds]);

  // Handle modal success
  const handleFundCreated = useCallback(() => {
    // Reload funds after creating a new one without showing loader
    fetchFunds(false);
  }, [fetchFunds]);

  // Render function for mobile fund cards - memoized to avoid recreation on every render
  const renderMobileFundCards = useCallback(() => {
    return (
      <Stack gap="md">
        {funds.map(fund => (
          <Card key={fund.id} padding="md" radius="md" withBorder>
            <Stack gap="xs">
              <Group gap="xs">
                <ThemeIcon size="sm" variant="light" radius="xl" color="teal">
                  <IconBuildingBank size="0.8rem" />
                </ThemeIcon>
                <Text fw={500}>{extractBankFromLabel(fund.label)}</Text>
              </Group>
              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Amount:
                </Text>
                <Text fw={500}>
                  {fund.baseCurrencySymbol} {formatNumber(fund.value, fund.baseCurrencyId)}
                </Text>
              </Group>
              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Currency:
                </Text>
                <Group gap="xs">
                  <ThemeIcon size="xs" variant="light" radius="xl" color="blue">
                    <Text size="xs" fw={700}>
                      {fund.baseCurrencySymbol || '-'}
                    </Text>
                  </ThemeIcon>
                  <Text>{fund.baseCurrency}</Text>
                </Group>
              </Group>
              {/* Only show conversion if currencies are different */}
              {fund.baseCurrency !== fund.defaultCurrency && (
                <Group justify="space-between">
                  <Text size="sm" c="dimmed">
                    Value in {fund.defaultCurrency}:
                  </Text>
                  <Text fw={500} c="blue">
                    {fund.defaultCurrencySymbol}{' '}
                    {formatNumber(fund.quoteCurrencyValue, fund.defaultCurrencyId)}
                  </Text>
                </Group>
              )}
            </Stack>
          </Card>
        ))}
        <Card padding="md" radius="md" withBorder>
          <Group justify="space-between">
            <Text fw={700}>Total:</Text>
            <Text fw={700}>
              {defaultCurrencyInfo.symbol}
              {formatNumber(totalValue, defaultCurrencyInfo.currencyId)}
            </Text>
          </Group>
        </Card>
      </Stack>
    );
  }, [funds, formatNumber, extractBankFromLabel, totalValue, defaultCurrencyInfo]);

  // Render function for desktop table
  const renderDesktopTable = useCallback(() => {
    return (
      <ScrollArea>
        <Table striped highlightOnHover withTableBorder>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>Amount</Table.Th>
              <Table.Th>Currency</Table.Th>
              <Table.Th>Account/Bank</Table.Th>
              <Table.Th>
                <Group gap="xs" wrap="nowrap">
                  <Text>Value in</Text>
                  {funds.length > 0 && (
                    <Group gap="xs">
                      <ThemeIcon size="sm" variant="light" radius="xl" color="blue">
                        <Text size="xs" fw={700}>
                          {defaultCurrencyInfo.symbol || '-'}
                        </Text>
                      </ThemeIcon>
                      <Text>{defaultCurrencyInfo.currency}</Text>
                    </Group>
                  )}
                </Group>
              </Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {funds.map(fund => (
              <Table.Tr key={fund.id}>
                <Table.Td fw={500}>{formatNumber(fund.value, fund.baseCurrencyId)}</Table.Td>
                <Table.Td>
                  <Group gap="xs">
                    <ThemeIcon size="sm" variant="light" radius="xl" color="blue">
                      <Text size="xs" fw={700}>
                        {fund.baseCurrencySymbol || '-'}
                      </Text>
                    </ThemeIcon>
                    <Text>{fund.baseCurrency}</Text>
                  </Group>
                </Table.Td>
                <Table.Td>
                  <Group gap="xs">
                    <ThemeIcon size="sm" variant="light" radius="xl" color="teal">
                      <IconBuildingBank size="0.8rem" />
                    </ThemeIcon>
                    <Text>{extractBankFromLabel(fund.label)}</Text>
                  </Group>
                </Table.Td>
                <Table.Td fw={500} c="blue">
                  {fund.defaultCurrencySymbol}{' '}
                  {formatNumber(fund.quoteCurrencyValue, fund.defaultCurrencyId)}
                </Table.Td>
              </Table.Tr>
            ))}
            <Table.Tr fw={700}>
              <Table.Td colSpan={3} ta="right">
                Total:
              </Table.Td>
              <Table.Td>
                {defaultCurrencyInfo.symbol}
                {formatNumber(totalValue, defaultCurrencyInfo.currencyId)}
              </Table.Td>
            </Table.Tr>
          </Table.Tbody>
        </Table>
      </ScrollArea>
    );
  }, [funds, formatNumber, extractBankFromLabel, totalValue, defaultCurrencyInfo]);

  // Memoize the content based on loading, error, and display mode
  const content = useMemo(() => {
    if (loading) {
      return (
        <Center p="xl">
          <Loader size="md" />
        </Center>
      );
    }

    if (error) {
      return (
        <Text c="red" ta="center" p="md">
          {error}
        </Text>
      );
    }

    return isMobile ? renderMobileFundCards() : renderDesktopTable();
  }, [loading, error, isMobile, renderMobileFundCards, renderDesktopTable]);

  return (
    <Box w="100%">
      <Stack gap="md">
        <Paper withBorder p="md" radius="md">
          <Stack gap="xs">
            <Title order={2}>Welcome, {user?.name}!</Title>
            {user?.email && <Text c="dimmed">Email: {user.email}</Text>}
          </Stack>
        </Paper>

        <Card shadow="sm" padding="lg" radius="md" withBorder>
          <Group gap="sm">
            <ThemeIcon color={fundCard.color} variant="light" size="lg" radius="md">
              {fundCard.icon}
            </ThemeIcon>
            <Title order={4}>{fundCard.title}</Title>
          </Group>
          <Text mt="sm" mb="md" size="sm" c="dimmed">
            {fundCard.description}
          </Text>

          <Button
            onClick={() => setCreateModalOpen(true)}
            leftSection={<IconPlus size="1rem" />}
            color="teal"
            variant="filled"
            fullWidth
            radius="md"
            mt="md"
            mb="xs"
          >
            Add New Fund
          </Button>

          <Divider my="md" />

          {content}
        </Card>
      </Stack>

      {/* Create Fund Modal */}
      <CreateFundModal
        opened={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        onSuccess={handleFundCreated}
      />
    </Box>
  );
};

export default FundsDashboard;
