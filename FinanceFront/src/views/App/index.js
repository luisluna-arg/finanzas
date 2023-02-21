import React from 'react';
import './index.css';
import './mui-grid.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-themes/dist/cable/index.min.css';

import {
    BrowserRouter,
    Routes,
    Route,
    NavLink
} from "react-router-dom";

import { URLs } from "./../../router/urls";

import Home from './../../components/Home';
import Dashboard from './../../components/Dashboard';
import Funds from './../../components/Funds/Funds';

function App() {
    const navLinkClass = 'flex-sm-fill text-sm-center nav-link ';
    const changeClass = ({ isActive }) => navLinkClass + (isActive ? 'active' : 'nav-link inactive');

    return (
        <div class="main">
            <BrowserRouter>
                <nav class="navbar navbar-expand-sm navbar-dark bg-success mb-3">
                    <div class="collapse navbar-collapse" id="navbar-0">
                        <div class="navbar-nav">
                            <NavLink to={URLs.Home} class="nav-item nav-link" className={changeClass}>Inicio</NavLink>
                            <NavLink to={URLs.Funds} class="nav-item nav-link" className={changeClass}>Fondos</NavLink>
                            <NavLink to={URLs.Dashboard} class="nav-item nav-link" className={changeClass}>Dashboard</NavLink>
                            <NavLink to={URLs.FCIFBARentaPesos} class="nav-item nav-link" className={changeClass}>FCI FBA Renta Pesos</NavLink>
                            <NavLink to={URLs.PlazosFijos} class="nav-item nav-link" className={changeClass}>Plazos Fijos</NavLink>
                            <NavLink to={URLs.MercadoPago} class="nav-item nav-link" className={changeClass}>Mercado Pago</NavLink>
                            <NavLink to={URLs.Lemon} class="nav-item nav-link" className={changeClass}>Lemon</NavLink>
                            <NavLink to={URLs.Dolar} class="nav-item nav-link" className={changeClass}>Dólar</NavLink>
                            <NavLink to={URLs.IOL} class="nav-item nav-link" className={changeClass}>IOL</NavLink>
                        </div>
                    </div>
                </nav>
                <Routes>
                    <Route path={URLs.Home} element={<Home />} />
                    <Route path={URLs.Funds} element={<Funds />} />
                    <Route path={URLs.Dashboard} element={<Dashboard />} />
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
