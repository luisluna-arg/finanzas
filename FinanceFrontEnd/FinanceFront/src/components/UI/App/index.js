import React from "react";
import { Container, Nav, Navbar } from "react-bootstrap";
import { QueryClient, QueryClientProvider } from "react-query";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom"; // Update import
import AdminDashboard from "../Admin/AdminDashboard";
import BankAccountMovements from "../Movements";
import CreditCardMovements from "../CreditCardMovements";
import Dashboard from "../Dashboard";
import Debits from "../Debits";
import Incomes from "../Incomes";
import Funds from "../Funds";
import Investments from "../Investments";
import CurrencyExchangeRates from "../CurrencyExchangeRates";
import "./App.scss";

const queryClient = new QueryClient();

function App() {
  const NavLink = ({ text, link }) => {
    return (
      <Nav.Link href={link}>
        {text}
      </Nav.Link>
    );
  };

  return (
    <div className="d-flex flex-column">
      <QueryClientProvider client={queryClient}>
        <Router>
          <Navbar variant="primary" className="text-dark">
            {/* <Navbar
            expand="lg"
            className={["bg-body-tertiary", "flex-shrink-1"]}
            style={{ height: "56px!important" }}
          > */}
            <Container>
              <Navbar.Brand href="#home">
                Finanzas
              </Navbar.Brand>
              <Navbar.Toggle aria-controls="basic-navbar-nav" />
              <Navbar.Collapse id="basic-navbar-nav">
                <Nav className={["me-auto"]}>
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
          </Navbar>

          <Routes>
            <Route path="/" exact element={<Dashboard />} />
            <Route path="/incomes" element={<Incomes />} />
            <Route path="/funds" element={<Funds />} />
            <Route path="/movements" element={<BankAccountMovements />} />
            <Route path="/credit-cards-movements" element={<CreditCardMovements />} />
            <Route path="/debits" element={<Debits />} />
            <Route path="/currency-exchange-rates" element={<CurrencyExchangeRates />} />
            <Route path="/investments" element={<Investments />} />
            <Route path="/admin" element={<AdminDashboard />} />
          </Routes>
        </Router>
      </QueryClientProvider>
    </div>
  );
}

export default App;
