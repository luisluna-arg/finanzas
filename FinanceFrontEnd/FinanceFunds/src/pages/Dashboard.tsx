import { useAuth } from "../auth";
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
} from "@mantine/core";
import { IconReceipt2, IconBuildingBank } from "@tabler/icons-react";
import { useState, useEffect } from "react";
import FundService from "../services/FundService";
import type { Fund } from "../services/types/FundTypes";

const Dashboard = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [funds, setFunds] = useState<Fund[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [isMobile, setIsMobile] = useState(window.innerWidth < 768);

  const fundCard = {
    title: "Funds",
    description: "Track your your available funds",
    icon: <IconReceipt2 size="1.5rem" stroke={1.5} />,
    color: "orange",
  };
  // Helper function to format number without currency symbol
  const formatNumber = (amount: number) => {
    return new Intl.NumberFormat("en-US", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(amount);
  };

  // Helper function to extract bank/account name from fund label
  const extractBankFromLabel = (label: string): string => {
    // Extract the bank/account name from the label
    // Assuming format remains similar but we'll just split by spaces
    const parts = label.split(" ");
    // Return all parts except the last one (which is likely the currency)
    return parts.slice(0, -1).join(" ").trim();
  };

  // Effect for media query
  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth < 768);
    };

    window.addEventListener("resize", handleResize);
    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  useEffect(() => {
    const fetchFunds = async () => {
      try {
        setLoading(true);
        // Fetch all funds without filtering
        const response = await FundService.getCurrentFunds();
        setFunds(response.items);
        setError(null);
      } catch (err) {
        console.error("Error fetching funds:", err);
        setError("Failed to load funds data. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchFunds();
  }, []);

  // Render function for mobile fund cards
  const renderMobileFundCards = () => {
    return (
      <Stack gap="md">
        {funds.map((fund) => (
          <Card key={fund.id} padding="md" radius="md" withBorder>
            <Stack gap="xs">
              {" "}
              <Group gap="xs">
                <ThemeIcon size="sm" variant="light" radius="xl" color="teal">
                  <IconBuildingBank size="0.8rem" />
                </ThemeIcon>
                <Text fw={500}>{extractBankFromLabel(fund.label)}</Text>
              </Group>{" "}
              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Amount:
                </Text>
                <Text fw={500}>
                  {fund.baseCurrencySymbol} {formatNumber(fund.value)}
                </Text>
              </Group>
              <Group justify="space-between">
                <Text size="sm" c="dimmed">
                  Currency:
                </Text>
                <Group gap="xs">
                  {" "}
                  <ThemeIcon size="xs" variant="light" radius="xl" color="blue">
                    <Text size="xs" fw={700}>
                      {fund.baseCurrencySymbol || "-"}
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
                    {fund.defaultCurrencySymbol}{" "}
                    {formatNumber(fund.quoteCurrencyValue)}
                  </Text>
                </Group>
              )}
            </Stack>
          </Card>
        ))}
        <Card padding="md" radius="md" withBorder>
          {" "}
          <Group justify="space-between">
            <Text fw={700}>Total:</Text>
            <Text fw={700}>
              {funds.length > 0 ? funds[0].defaultCurrencySymbol : ""}{" "}
              {formatNumber(
                funds.reduce(
                  (total, fund) => total + fund.quoteCurrencyValue,
                  0,
                ),
              )}
            </Text>
          </Group>
        </Card>
      </Stack>
    );
  };

  // Render function for desktop table
  const renderDesktopTable = () => {
    return (
      <ScrollArea>
        <Table striped highlightOnHover withTableBorder>
          {" "}
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
                      {" "}
                      <ThemeIcon
                        size="sm"
                        variant="light"
                        radius="xl"
                        color="blue"
                      >
                        <Text size="xs" fw={700}>
                          {funds[0].defaultCurrencySymbol || "-"}
                        </Text>
                      </ThemeIcon>
                      <Text>{funds[0].defaultCurrency}</Text>
                    </Group>
                  )}
                </Group>
              </Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {funds.map((fund) => (
              <Table.Tr key={fund.id}>
                <Table.Td fw={500}>{formatNumber(fund.value)}</Table.Td>{" "}
                <Table.Td>
                  <Group gap="xs">
                    <ThemeIcon
                      size="sm"
                      variant="light"
                      radius="xl"
                      color="blue"
                    >
                      <Text size="xs" fw={700}>
                        {fund.baseCurrencySymbol || "-"}
                      </Text>
                    </ThemeIcon>
                    <Text>{fund.baseCurrency}</Text>
                  </Group>
                </Table.Td>
                <Table.Td>
                  <Group gap="xs">
                    <ThemeIcon
                      size="sm"
                      variant="light"
                      radius="xl"
                      color="teal"
                    >
                      <IconBuildingBank size="0.8rem" />
                    </ThemeIcon>
                    <Text>{extractBankFromLabel(fund.label)}</Text>
                  </Group>
                </Table.Td>{" "}
                <Table.Td
                  fw={500}
                  c={
                    fund.baseCurrency !== fund.defaultCurrency
                      ? "blue"
                      : undefined
                  }
                >
                  {formatNumber(fund.quoteCurrencyValue)}
                </Table.Td>
              </Table.Tr>
            ))}{" "}
            <Table.Tr fw={700}>
              <Table.Td colSpan={3} ta="right">
                Total:
              </Table.Td>
              <Table.Td>
                {formatNumber(
                  funds.reduce(
                    (total, fund) => total + fund.quoteCurrencyValue,
                    0,
                  ),
                )}
              </Table.Td>
            </Table.Tr>
          </Table.Tbody>
        </Table>
      </ScrollArea>
    );
  };

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
            <ThemeIcon
              color={fundCard.color}
              variant="light"
              size="lg"
              radius="md"
            >
              {fundCard.icon}
            </ThemeIcon>
            <Title order={4}>{fundCard.title}</Title>
          </Group>
          <Text mt="sm" mb="md" size="sm" c="dimmed">
            {fundCard.description}
          </Text>

          <Divider my="md" />

          {loading ? (
            <Center p="xl">
              <Loader size="md" />
            </Center>
          ) : error ? (
            <Text c="red" ta="center" p="md">
              {error}
            </Text>
          ) : isMobile ? (
            renderMobileFundCards()
          ) : (
            renderDesktopTable()
          )}
        </Card>
      </Stack>
    </Box>
  );
};

export default Dashboard;
