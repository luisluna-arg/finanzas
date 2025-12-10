import { useLocation } from "react-router";
import { useEffect, useState } from "react";
import {
    NavigationMenu,
    NavigationMenuItem,
    NavigationMenuLink,
    NavigationMenuList,
    navigationMenuTriggerStyle,
} from "@/components/ui/shadcn/navigation-menu";
import { Button } from "@/components/ui/shadcn/button";
import { cn } from "@/lib/utils";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMoon, faSun } from "@fortawesome/free-solid-svg-icons";

const NavLink = ({ text, link }: { text: string; link: string }) => {
    const location = useLocation();
    const isActive = location.pathname === link || location.pathname.startsWith(link + "/");
    
    return (
        <NavigationMenuItem>
            <NavigationMenuLink
                href={link}
                className={cn(
                    navigationMenuTriggerStyle(),
                    isActive && "bg-gray-100 text-gray-900 dark:bg-gray-800 dark:text-gray-50"
                )}
            >
                {text}
            </NavigationMenuLink>
        </NavigationMenuItem>
    );
};

export default function Navigation() {
    const [isDarkMode, setIsDarkMode] = useState(false);

    useEffect(() => {
        // Check localStorage and current document state for dark mode
        const storedTheme = localStorage.getItem('theme');
        const isDark = storedTheme === 'dark' || document.documentElement.classList.contains('dark');
        setIsDarkMode(isDark);
    }, []);

    const toggleDarkMode = () => {
        const newDarkMode = !isDarkMode;
        setIsDarkMode(newDarkMode);
        
        if (newDarkMode) {
            document.documentElement.classList.add('dark');
            localStorage.setItem('theme', 'dark');
        } else {
            document.documentElement.classList.remove('dark');
            localStorage.setItem('theme', 'light');
        }
    };

    return (
        <div className="flex justify-center bg-secondary text-secondary-foreground px-2 py-1">
            <NavigationMenu>
                <NavigationMenuList>
                    <NavLink text="Dashboard" link="/" />
                    <NavLink text="Ingresos" link="/incomes" />
                    <NavLink text="Fondos" link="/funds" />
                    <NavLink text="Suscripciones" link="/subscriptions" />
                    <NavLink text="Débitos" link="/debits" />
                    <NavLink text="Movimientos" link="/movements" />
                    <NavLink
                        text="Tarjetas de crédito"
                        link="/credit-cards-movements"
                    />
                    <NavLink
                        text="Cotizaciones"
                        link="/currency-exchange-rates"
                    />
                    <NavLink text="Inversiones" link="/investments" />
                    <NavLink text="Administración" link="/admin" />
                    <NavigationMenuItem>
                        <Button
                            variant="ghost"
                            size="sm"
                            onClick={toggleDarkMode}
                            className={cn(navigationMenuTriggerStyle(), "cursor-pointer")}
                        >
                            <FontAwesomeIcon icon={isDarkMode ? faSun : faMoon} />
                        </Button>
                    </NavigationMenuItem>
                </NavigationMenuList>
            </NavigationMenu>
        </div>
    );
}
