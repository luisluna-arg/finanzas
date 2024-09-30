import React from "react";

const ActionLink: React.FC<{
  text: string;
  action: () => void;
  isActive?: boolean;
  isEnabled?: boolean;
  classes?: string[];
}> = ({ text, action, isActive = false, isEnabled = false, classes }) => {
  let localClasses = ["page-link", ...(classes ?? [])];
  if (isActive)
  {
    localClasses.push("active")
  }

  console.log("text:", text, "isEnabled", isEnabled);

  return (
    <a className={localClasses?.join(" ")} href="#" onClick={action} aria-disabled={!isEnabled}>{text}</a>
  );
}

export default ActionLink;
