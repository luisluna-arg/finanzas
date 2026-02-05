import React from "react";
import Picker from "@/components/ui/utils/Picker";
import { cn } from "@/lib/utils";

interface PickerData {
    id: string;
    name: string;
}

interface BankCurrencySelectorProps {
    banks: PickerData[];
    currencies: PickerData[];
    bankId: string;
    currencyId: string;
    onBankChange: (picker: { value: string }) => void;
    onCurrencyChange: (picker: { value: string }) => void;
}

const BankCurrencySelector: React.FC<BankCurrencySelectorProps> = ({
    banks,
    currencies,
    bankId,
    currencyId,
    onBankChange,
    onCurrencyChange,
}) => {
    return (
        <>
            <div className="flex flex-row justify-center gap-10">
                <Picker
                    id="bank-picker"
                    placeholder="Seleccionar banco..."
                    value={bankId}
                    data={banks}
                    mapper={{
                        id: "id",
                        label: (record: unknown) =>
                            `${(record as PickerData).name}`,
                    }}
                    onChange={onBankChange}
                    className={"w-60"}
                />
                <Picker
                    id="currency-picker"
                    placeholder="Seleccionar moneda..."
                    value={currencyId}
                    data={currencies}
                    mapper={{
                        id: "id",
                        label: (record: unknown) =>
                            `${(record as PickerData).name}`,
                    }}
                    onChange={onCurrencyChange}
                    className={"w-60"}
                />
            </div>
            <hr className={cn("py-1", "mb-5", "mt-5")} />
        </>
    );
};

export default BankCurrencySelector;
