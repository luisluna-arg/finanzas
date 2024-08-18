import { Container, Nav, Navbar } from "react-bootstrap";

const NavLink = ({ text, link }: { text: string, link: string }) => {
    return (
        <Nav.Link href={link}>
            {text}
        </Nav.Link>
    );
};

export default function Navigation() {
    return (<Navbar variant="primary" className="text-dark">
        <Container>
            <Navbar.Brand href="#home">
                Finanzas
            </Navbar.Brand>
            <Navbar.Toggle aria-controls="basic-navbar-nav" />
            <Navbar.Collapse id="basic-navbar-nav">
                <Nav className={"me-auto"}>
                    <NavLink text="Dashboard" link="/" />
                    <NavLink text="Ingresos" link="/incomes" />
                    <NavLink text="Fondos" link="/funds" />
                    <NavLink text="Movimientos" link="/movements" />
                    <NavLink text="Tarjetas de crédito" link="/credit-cards-movements" />
                    <NavLink text="Débitos" link="/debits" />
                    <NavLink text="Cotizaciones" link="/currency-exchange-rates" />
                    <NavLink text="Inversiones" link="/investments" />
                    <NavLink text="Administración" link="/admin" />
                </Nav>
            </Navbar.Collapse>
        </Container>
    </Navbar>);
}