import React from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import {
  BrowserRouter,
  Routes,
  Route,
  NavLink
} from "react-router-dom";

import { URLs } from "./router/urls";

import Home from './components/Home/Home';
import Dashboard from './components/Dashboard/Dashboard';
import Funds from './components/funds/Funds/Funds';


function App() {
  const navLinkClass = 'flex-sm-fill text-sm-center nav-link ';
  const changeClass = ({ isActive }) => navLinkClass + (isActive ? 'active' : 'nav-link inactive');

  return (
    <div>
      <BrowserRouter>
        <ul className="nav nav-tabs">
          <li className="nav-item">
            <NavLink to={URLs.Home} className={changeClass}>Inicio</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.Dashboard} className={changeClass}>Dashboard</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.Funds} className={changeClass}>Fondos</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.FCIFBARentaPesos} className={changeClass}>FCI FBA Renta Pesos</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.PlazosFijos} className={changeClass}>Plazos Fijos</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.MercadoPago} className={changeClass}>Mercado Pago</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.Lemon} className={changeClass}>Lemon</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.Dolar} className={changeClass}>Dólar</NavLink>
          </li>
          <li className="nav-item">
            <NavLink to={URLs.IOL} className={changeClass}>IOL</NavLink>
          </li>
        </ul>
        <Routes>
          <Route path={URLs.Home} element={<Home />} />
          <Route path={URLs.Dashboard} element={<Dashboard />} />
          <Route path={URLs.Funds} element={<Funds />} />
          {/* 
        <Route path='/about' element={<About />} />
        <Route path='/dolar' element={<Dolar />} />
        <Route path='/fciRentaPesos' element={<FCIRentaPesos />} /> 
        <Route path='/iol' element={<IOL />} />
        <Route path='/lemon' element={<Lemon />} />
        <Route path='/mercadoPago' element={<MercadoPago />} />
        <Route path='/plazosFijos' element={<PlazosFijos />} /> 
        */}
        </Routes>
      </BrowserRouter>
    </div >
  );
}

export default App;
