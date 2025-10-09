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
import { IconCurrencyDollar, IconPlus } from '@tabler/icons-react';
import { useState, useEffect, useCallback, useMemo } from 'react';
import SafeLogger from '@/utils/SafeLogger';
import CurrencyExchangeRateService from '../services/CurrencyExchangeRateService';
import type { CurrencyExchangeRate } from '../services/types/CurrencyExchangeRateTypes';
import CreateExchangeRateModal from '../components/CreateExchangeRateModal';

const CurrencyExchangeDashboard = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [exchangeRates, setExchangeRates] = useState<CurrencyExchangeRate[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);
  const [createModalOpen, setCreateModalOpen] = useState(false);

  // Memoized card configuration to avoid recreation
  const exchangeRateCard = useMemo(
    () => ({
      title: 'Currency Exchange Rates',
      description: 'Manage your currency conversion rates',
      icon: <IconCurrencyDollar size="1.5rem" stroke={1.5} />,
      color: 'blue',
    }),
    []
  );

  // Helper function to format exchange rate with proper precision
  const formatRate = useCallback((rate: number) => {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 8,
    }).format(rate);
  }, []);

  // Helper function to format timestamp
  const formatTimestamp = useCallback((timestamp: string) => {
    return new Intl.DateTimeFormat('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(new Date(timestamp));
  }, []);

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

  // Fetch exchange rates data
  const fetchExchangeRates = useCallback(async (showLoading = true) => {
    if (showLoading) setLoading(true);

    try {
      // Request latest exchange rates
      const response = await CurrencyExchangeRateService.getLatestExchangeRates({}, true);

      // Update state in the next frame to avoid blocking the main thread
      setTimeout(() => {
        SafeLogger.info('Fetched exchange rates:', response);
        setExchangeRates(response || []);
        setError(null);
        if (showLoading) setLoading(false);
      }, 0);
    } catch (err) {
      SafeLogger.error('Error fetching exchange rates:', err);

      setTimeout(() => {
        setError('Failed to load exchange rates data. Please try again later.');
        if (showLoading) setLoading(false);
      }, 0);
    }
  }, []);

  // Initial data fetch
  useEffect(() => {
    fetchExchangeRates();
  }, [fetchExchangeRates]);

  // Handle modal success
  const handleExchangeRateCreated = useCallback(() => {
    // Reload exchange rates after creating a new one without showing loader
    fetchExchangeRates(false);
  }, [fetchExchangeRates]);

  // Render function for mobile exchange rate cards - memoized to avoid recreation
  const renderMobileExchangeRateCards = useCallback(() => {
    return (
      <Stack gap="md">
        {exchangeRates.map(rate => (
          <Card key={rate.id} padding="md" radius="md" withBorder>
            <Stack gap="xs">
              <Group justify="space-between">
                <Group gap="xs">
                  <ThemeIcon size="sm" variant="light" radius="xl" color="blue">
                    <IconCurrencyDollar size="0.8rem" />
                  </ThemeIcon>
                  <Text fw={500}>
                    {rate.baseCurrency.shortName} → {rate.quoteCurrency.shortName}
                  </Text>
                </Group>
              </Group>

              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Buy Rate:
                </Text>
                <Text fw={500} c="green">
                  {formatRate(rate.buyRate.value)}
                </Text>
              </Group>

              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Sell Rate:
                </Text>
                <Text fw={500} c="red">
                  {formatRate(rate.sellRate.value)}
                </Text>
              </Group>

              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Last Updated:
                </Text>
                <Text size="sm">{formatTimestamp(rate.timeStamp)}</Text>
              </Group>
            </Stack>
          </Card>
        ))}
        {exchangeRates.length === 0 && !loading && (
          <Card padding="md" radius="md" withBorder>
            <Text ta="center" c="dimmed">
              No exchange rates found. Add your first exchange rate to get started.
            </Text>
          </Card>
        )}
      </Stack>
    );
  }, [exchangeRates, formatRate, formatTimestamp, loading]);

  // Render function for desktop table
  const renderDesktopTable = useCallback(() => {
    return (
      <ScrollArea>
        <Table striped highlightOnHover withTableBorder>
          <Table.Thead>
            <Table.Tr>
              <Table.Th>Pair</Table.Th>
              <Table.Th>Buy Rate</Table.Th>
              <Table.Th>Sell Rate</Table.Th>
              <Table.Th>Last Updated</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {exchangeRates.map(rate => (
              <Table.Tr key={rate.id}>
                <Table.Td>
                  <Group gap="xs">
                    <ThemeIcon size="sm" variant="light" radius="xl" color="blue">
                      <IconCurrencyDollar size="0.8rem" />
                    </ThemeIcon>
                    <Text fw={500}>
                      {rate.baseCurrency.shortName} → {rate.quoteCurrency.shortName}
                    </Text>
                  </Group>
                </Table.Td>
                <Table.Td>
                  <Text fw={500} c="green">
                    {formatRate(rate.buyRate.value)}
                  </Text>
                </Table.Td>
                <Table.Td>
                  <Text fw={500} c="red">
                    {formatRate(rate.sellRate.value)}
                  </Text>
                </Table.Td>
                <Table.Td>
                  <Text size="sm">{formatTimestamp(rate.timeStamp)}</Text>
                </Table.Td>
              </Table.Tr>
            ))}
          </Table.Tbody>
        </Table>
        {exchangeRates.length === 0 && !loading && (
          <Center p="xl">
            <Text c="dimmed">
              No exchange rates found. Add your first exchange rate to get started.
            </Text>
          </Center>
        )}
      </ScrollArea>
    );
  }, [exchangeRates, formatRate, formatTimestamp, loading]);

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

    return isMobile ? renderMobileExchangeRateCards() : renderDesktopTable();
  }, [loading, error, isMobile, renderMobileExchangeRateCards, renderDesktopTable]);

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
            <ThemeIcon color={exchangeRateCard.color} variant="light" size="lg" radius="md">
              {exchangeRateCard.icon}
            </ThemeIcon>
            <Title order={4}>{exchangeRateCard.title}</Title>
          </Group>
          <Text mt="sm" mb="md" size="sm" c="dimmed">
            {exchangeRateCard.description}
          </Text>

          <Button
            onClick={() => setCreateModalOpen(true)}
            leftSection={<IconPlus size="1rem" />}
            color="blue"
            variant="filled"
            fullWidth
            radius="md"
            mt="md"
            mb="xs"
          >
            Add New Exchange Rate
          </Button>

          <Divider my="md" />

          {content}
        </Card>
      </Stack>

      {/* Create Exchange Rate Modal */}
      <CreateExchangeRateModal
        opened={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        onSuccess={handleExchangeRateCreated}
      />
    </Box>
  );
};

export default CurrencyExchangeDashboard;
