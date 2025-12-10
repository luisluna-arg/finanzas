import React from "react";
import ActionLink from "@/components/ui/utils/ActionLink";

/* TODO Fix the ammount of page pickers displayed on screen */
const PaginationBar: React.FC<{
    page: number;
    totalPages: number;
    action: (page: number) => void;
}> = ({ page, totalPages, action }) => {
    const previousPageEnabled = page > 1;
    const nextPageEnabled = page < totalPages;

    return (
        <ul className="pagination">
            <li
                className={
                    "page-item" + (!previousPageEnabled ? " disabled" : "")
                }
            >
                <ActionLink
                    text="Anterior"
                    action={() => action(page - 1)}
                    isEnabled={previousPageEnabled}
                />
            </li>
            {Array.from({ length: totalPages }, (_, i) => {
                const isActive = i === page - 1 ? " active" : "";
                return (
                    <li key={i} className="page-item">
                        <button
                            type="button"
                            className={"page-link" + isActive}
                            onClick={() => action(i)}
                        >
                            {i + 1}
                        </button>
                    </li>
                );
            })}
            <li className={"page-item" + (!nextPageEnabled ? " disabled" : "")}>
                <ActionLink
                    text="Siguiente"
                    action={() => action(page + 1)}
                    isEnabled={nextPageEnabled}
                />
            </li>
        </ul>
    );
};

export default PaginationBar;
