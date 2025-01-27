type NavProps = {
  className?: string | string[];
  children?: any;
};

export const Nav: React.FC<NavProps> = ({ children }) => {
  return <>{children}</>;
};

type NavlinkProps = {
  href?: string;
  children?: any;
};

export const Navlink: React.FC<NavlinkProps> = ({ href, children }) => {
  return <>{children}</>;
};

type NavbarProps = {
  className?: string | string[];
  variant?: string;
  children?: any;
};

export const Navbar: React.FC<NavbarProps> = ({ variant, children }) => {
  return <>{children}</>;
};

type NavbarCollapseProps = {
  id?: string;
  className?: string | string[];
  variant?: string;
  children?: any;
};

export const NavbarCollapse: React.FC<NavbarCollapseProps> = ({
  variant,
  children,
}) => {
  return <>{children}</>;
};

type NavbarToggleProps = {
  ariaControls?: string;
  children?: any;
};

export const NavbarToggle: React.FC<NavbarToggleProps> = ({
  ariaControls,
  children,
}) => {
  return <>{children}</>;
};

type NavbarBrandProps = {
  href?: string;
  children?: any;
};

export const NavbarBrand: React.FC<NavbarBrandProps> = ({ href, children }) => {
  return <>{children}</>;
};
