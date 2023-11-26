import React, { useState } from "react";
import { Col, Nav, Row, Tab } from "react-bootstrap";
import Bank from "../Bank";
import CreditCardIssuer from "../CreditCardIssuer";

const componentMap = {
  Bank: <Bank />,
  CreditCardIssuer: <CreditCardIssuer />,
};

const labelMap = {
  Bank: "Bancos",
  CreditCardIssuer: "Emisores de Tarjetas de Crédito",
};

const AdminDashboard = () => {
  const [activeTab, setActiveTab] = useState("Bank");

  const handleTabClick = (tab) => {
    setActiveTab(tab);
  };

  return (
    <Tab.Container id="tabs" activeKey={activeTab}>
      <Row className="h-100 me-0">
        <Col className="me-0 col-md-2 pe-0 text-bg-dark text-light">
          <div className="d-flex flex-column flex-shrink-0 p-3">
            <span className="fs-4">Módulos</span>
            <hr />
            <Nav
              variant="pills"
              className="flex-column mb-auto"
              defaultActiveKey="/home"
            >
              {Object.keys(componentMap).map((tab) => (
                <Nav.Item key={tab}>
                  <Nav.Link
                    className="text-light"
                    eventKey={tab}
                    onClick={() => handleTabClick(tab)}
                  >
                    {`${labelMap[tab]}`}{" "}
                  </Nav.Link>
                </Nav.Item>
              ))}
              {/* <Nav.Item>
                <Nav.Link eventKey="disabled" disabled>
                  Disabled
                </Nav.Link>
              </Nav.Item> */}
            </Nav>
          </div>
        </Col>
        <Col className="col-md-10 text-bg-light">
          {/* Your content area */}
          {componentMap[activeTab]}
        </Col>
      </Row>
    </Tab.Container>
  );
};

export default AdminDashboard;
