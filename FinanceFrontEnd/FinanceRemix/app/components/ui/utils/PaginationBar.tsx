import React, { useState } from "react";
import ActionLink from "@/app/components/ui/utils/ActionLink";

/* TODO Fix the ammount of page pickers displayed on screen */
const PaginationBar: React.FC<{
  page: number;
  totalPages: number;
  action: (page: number) => void;
}> = ({ page, totalPages, action }) => {
  const [previousPageEnabled, setPreviousPageEnabled] = useState<boolean>(page > 1);
  const [nextPageEnabled, setNextPageEnabled] = useState<boolean>(page < totalPages);

  return (
    <ul className="pagination">
      <li className={"page-item" + (!previousPageEnabled ? " disabled" : "")}>
        <ActionLink
          text="Anterior"
          action={(e) => action(page - 1)}
          isEnabled={previousPageEnabled}
        />
      </li>
      {Array.from({ length: totalPages }, (_, i) => {
        let isActive = i === page - 1 ? " active" : "";
        return (
          <li key={i} className="page-item">
            <a className={"page-link" + isActive} href="#" onClick={() => action(i)}>{i + 1}</a>
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
}

export default PaginationBar;
