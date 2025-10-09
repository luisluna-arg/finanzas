import React from "react";

const ActionLink: React.FC<{
    text: string;
    action: (e?: React.MouseEvent<HTMLButtonElement>) => void;
    isActive?: boolean;
    isEnabled?: boolean;
    classes?: string[];
}> = ({ text, action, isActive = false, isEnabled = false, classes }) => {
    const localClasses = ["page-link", ...(classes ?? [])];
    if (isActive) {
        localClasses.push("active");
    }
    function onClick(e: React.MouseEvent<HTMLButtonElement>) {
        e.preventDefault();
        action(e);
    }

    return (
        <button
            className={localClasses?.join(" ")}
            onClick={onClick}
            aria-disabled={!isEnabled}
            disabled={!isEnabled}
            type="button"
        >
            {text}
        </button>
    );
};

export default ActionLink;
