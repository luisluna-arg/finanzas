import React from "react";
import { Container, Nav, Navbar } from "react-bootstrap";
import { QueryClient, QueryClientProvider } from "react-query";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom"; // Update import
import About from "../About";
import Home from "../Home";
import BankAccountMovements from "../Movements/List";
import "./App.css";

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Navbar expand="lg" className={["bg-success", "text-light"]}>
          <Container>
            <Navbar.Brand className="text-light" href="#home">
              Finanzas
            </Navbar.Brand>
            <Navbar.Toggle aria-controls="basic-navbar-nav" />
            <Navbar.Collapse id="basic-navbar-nav">
              <Nav className={["me-auto"]}>
                <Nav.Link className="text-light" href="/">
                  Dashboard
                </Nav.Link>
                <Nav.Link className="text-light" href="/movements">
                  Movimientos
                </Nav.Link>
              </Nav>
            </Navbar.Collapse>
          </Container>
        </Navbar>

        <Routes>
          <Route path="/" exact element={<Home />} />
          <Route path="/movements" element={<BankAccountMovements />} />
          <Route path="/about" element={<About />} />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
