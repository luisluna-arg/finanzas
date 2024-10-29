import React from "react";

const ActionLink: React.FC<{
  text: string;
  action: (e?: any) => void;
  isActive?: boolean;
  isEnabled?: boolean;
  classes?: string[];
}> = ({ text, action, isActive = false, isEnabled = false, classes }) => {
  let localClasses = ["page-link", ...(classes ?? [])];
  if (isActive)
  {
    localClasses.push("active")
  }

  function onClick(e: any) {
    e.preventDefault();
    action();
  }

  return (
    <a className={localClasses?.join(" ")} href="#" onClick={onClick} aria-disabled={!isEnabled}>{text}</a>
  );
}

export default ActionLink;
