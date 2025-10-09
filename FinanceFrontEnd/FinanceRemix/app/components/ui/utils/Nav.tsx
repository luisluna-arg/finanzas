type NavProps = {
    className?: string | string[];
    children?: any;
};

export const Nav: React.FC<NavProps> = ({ children }) => {
    return <>{children}</>;
};

type NavlinkProps = {
    children?: any;
};

export const Navlink: React.FC<NavlinkProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarProps = {
    className?: string | string[];
    children?: any;
};

export const Navbar: React.FC<NavbarProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarCollapseProps = {
    id?: string;
    className?: string | string[];
    children?: any;
};

export const NavbarCollapse: React.FC<NavbarCollapseProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarToggleProps = {
    ariaControls?: string;
    children?: any;
};

export const NavbarToggle: React.FC<NavbarToggleProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarBrandProps = {
    children?: any;
};

export const NavbarBrand: React.FC<NavbarBrandProps> = ({ children }) => {
    return <>{children}</>;
};
