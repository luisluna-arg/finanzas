/* eslint-disable react/prop-types */

type NavProps = {
    className?: string | string[];
    children?: React.ReactNode;
};

export const Nav: React.FC<NavProps> = ({ children }) => {
    return <>{children}</>;
};

type NavlinkProps = {
    children?: React.ReactNode;
};

export const Navlink: React.FC<NavlinkProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarProps = {
    className?: string | string[];
    children?: React.ReactNode;
};

export const Navbar: React.FC<NavbarProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarCollapseProps = {
    id?: string;
    className?: string | string[];
    children?: React.ReactNode;
};

export const NavbarCollapse: React.FC<NavbarCollapseProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarToggleProps = {
    ariaControls?: string;
    children?: React.ReactNode;
};

export const NavbarToggle: React.FC<NavbarToggleProps> = ({ children }) => {
    return <>{children}</>;
};

type NavbarBrandProps = {
    children?: React.ReactNode;
};

export const NavbarBrand: React.FC<NavbarBrandProps> = ({ children }) => {
    return <>{children}</>;
};
