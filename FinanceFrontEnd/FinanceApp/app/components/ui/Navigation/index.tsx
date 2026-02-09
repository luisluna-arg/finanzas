import { useLocation } from "react-router";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/shadcn/button";
import { cn } from "@/lib/utils";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMoon, faSun } from "@fortawesome/free-solid-svg-icons";

const NavLink = ({ text, link }: { text: string; link: string }) => {
    const location = useLocation();
    const isActive = location.pathname === link || location.pathname.startsWith(link + "/");
    
    return (
        <a
            href={link}
            className={cn(
                "px-4 py-2 text-sm font-medium transition-colors hover:text-foreground/80",
                isActive ? "text-foreground" : "text-foreground/60"
            )}
        >
            {text}
        </a>
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
        <nav className="flex items-center justify-center bg-secondary text-secondary-foreground px-2 py-1 gap-1">
            <NavLink text="Dashboard" link="/" />
            <NavLink text="Ingresos" link="/incomes" />
            <NavLink text="Fondos" link="/funds" />
            <NavLink text="Suscripciones" link="/subscriptions" />
            {/* <NavLink text="Débitos" link="/debits" /> */}
            {/* <NavLink text="Movimientos" link="/movements" /> */}
            <NavLink
                text="Tarjetas de crédito"
                link="/credit-cards"
            />
            <NavLink
                text="Cotizaciones"
                link="/currency-exchange-rates"
            />
            <NavLink text="Inversiones" link="/investments" />
            <NavLink text="Administración" link="/admin" />
            <Button
                variant="ghost"
                size="sm"
                onClick={toggleDarkMode}
                className="cursor-pointer h-10 ml-2"
            >
                <FontAwesomeIcon icon={isDarkMode ? faSun : faMoon} />
            </Button>
        </nav>
    );
}
