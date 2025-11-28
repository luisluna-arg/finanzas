import { useLocation } from "@remix-run/react";
import {
    NavigationMenu,
    NavigationMenuItem,
    NavigationMenuLink,
    NavigationMenuList,
    navigationMenuTriggerStyle,
} from "@/components/ui/shadcn/navigation-menu";
import { cn } from "@/lib/utils";

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
                </NavigationMenuList>
            </NavigationMenu>
        </div>
    );
}
