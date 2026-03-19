import React from "react";
import {
    ChevronsLeftIcon,
    ChevronsRightIcon,
    ChevronLeftIcon,
    ChevronRightIcon,
} from "lucide-react";
import {
    Pagination,
    PaginationContent,
    PaginationItem,
} from "@/components/ui/shadcn/pagination";
import { buttonVariants } from "@/components/ui/shadcn/button";
import { cn } from "@/lib/utils";

const PaginationBar: React.FC<{
    page: number;
    totalPages: number;
    action: (page: number) => void;
}> = ({ page, totalPages, action }) => {
    const atFirst = page <= 1;
    const atLast = page >= totalPages;

    function handlePageInput(e: React.ChangeEvent<HTMLInputElement>) {
        const value = parseInt(e.target.value, 10);
        if (!isNaN(value) && value >= 1 && value <= totalPages) {
            action(value);
        }
    }

    const navBtn = (disabled: boolean) =>
        cn(buttonVariants({ variant: "ghost", size: "icon" }), disabled && "pointer-events-none opacity-50");

    return (
        <Pagination>
            <PaginationContent>
                <PaginationItem>
                    <button
                        type="button"
                        aria-label="Primera página"
                        className={navBtn(atFirst)}
                        disabled={atFirst}
                        onClick={() => action(1)}
                    >
                        <ChevronsLeftIcon className="size-4" />
                    </button>
                </PaginationItem>

                <PaginationItem>
                    <button
                        type="button"
                        aria-label="Página anterior"
                        className={navBtn(atFirst)}
                        disabled={atFirst}
                        onClick={() => action(page - 1)}
                    >
                        <ChevronLeftIcon className="size-4" />
                    </button>
                </PaginationItem>

                <PaginationItem className="flex items-center gap-2 px-1">
                    <input
                        type="number"
                        min={1}
                        max={totalPages}
                        value={page}
                        onChange={handlePageInput}
                        aria-label="Página"
                        className="w-14 rounded-md border border-input bg-background px-2 py-1 text-center text-sm shadow-sm focus:outline-none focus:ring-1 focus:ring-ring"
                    />
                    <span className="whitespace-nowrap text-sm text-muted-foreground">
                        de {totalPages}
                    </span>
                </PaginationItem>

                <PaginationItem>
                    <button
                        type="button"
                        aria-label="Página siguiente"
                        className={navBtn(atLast)}
                        disabled={atLast}
                        onClick={() => action(page + 1)}
                    >
                        <ChevronRightIcon className="size-4" />
                    </button>
                </PaginationItem>

                <PaginationItem>
                    <button
                        type="button"
                        aria-label="Última página"
                        className={navBtn(atLast)}
                        disabled={atLast}
                        onClick={() => action(totalPages)}
                    >
                        <ChevronsRightIcon className="size-4" />
                    </button>
                </PaginationItem>
            </PaginationContent>
        </Pagination>
    );
};

export default PaginationBar;
