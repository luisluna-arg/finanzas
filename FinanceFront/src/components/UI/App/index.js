import React from "react";
import { Container, Nav, Navbar } from "react-bootstrap";
import { QueryClient, QueryClientProvider } from "react-query";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom"; // Update import
import About from "../About";
import AdminDashboard from "../Admin/AdminDashboard";
import Dashboard from "../Dashboard";
import BankAccountMovements from "../Movements/List";
import "./App.css";

const queryClient = new QueryClient();

function App() {
  return (
    <div className="d-flex flex-column vh-100">
      <QueryClientProvider client={queryClient}>
        <Router>
          <Navbar
            expand="lg"
            className={["bg-success", "text-light", "flex-shrink-1"]}
            style={{ height: "56px!important" }}
          >
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
                  <Nav.Link className="text-light" href="/admin">
                    Administración
                  </Nav.Link>
                </Nav>
              </Navbar.Collapse>
            </Container>
          </Navbar>

          <Routes>
            <Route path="/" exact element={<Dashboard />} />
            <Route path="/movements" element={<BankAccountMovements />} />
            <Route path="/admin" element={<AdminDashboard />} />
            <Route path="/about" element={<About />} />
          </Routes>
        </Router>
      </QueryClientProvider>
    </div>
  );
}

export default App;
