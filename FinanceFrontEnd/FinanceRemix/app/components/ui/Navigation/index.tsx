import {
    NavigationMenu,
    NavigationMenuItem,
    NavigationMenuLink,
    NavigationMenuList,
    navigationMenuTriggerStyle,
} from "@/components/ui/shadcn/navigation-menu";

const NavLink = ({ text, link }: { text: string; link: string }) => {
    return (
        <NavigationMenuItem>
            <NavigationMenuLink
                href={link}
                className={navigationMenuTriggerStyle()}
            >
                {text}
            </NavigationMenuLink>
        </NavigationMenuItem>
    );
};

export default function Navigation() {
    return (
        <div className="flex justify-center">
            <NavigationMenu>
                <NavigationMenuList>
                    <NavLink text="Dashboard" link="/" />
                    <NavLink text="Ingresos" link="/incomes" />
                    <NavLink text="Fondos" link="/funds" />
                    <NavLink text="Movimientos" link="/movements" />
                    <NavLink
                        text="Tarjetas de crédito"
                        link="/credit-cards-movements"
                    />
                    <NavLink text="Débitos" link="/debits" />
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
