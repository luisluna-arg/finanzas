import React from 'react';
import './index.css';
import './mui-grid.css';
import './popup.css';
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
import { Provider } from '../../context';

function App() {
    const navLinkClass = 'flex-sm-fill text-sm-center nav-link ';
    const changeClass = ({ isActive }) => navLinkClass + (isActive ? 'active' : 'nav-link inactive');

    return (
        <div className="main">
            <Provider>
                <BrowserRouter>
                    <nav className="navbar navbar-expand-sm navbar-dark bg-success mb-3">
                        <div className="collapse navbar-collapse" id="navbar-0">
                            <div className="navbar-nav">
                                <NavLink to={URLs.Home} className={"nav-item nav-link " + changeClass}>Inicio</NavLink>
                                <NavLink to={URLs.Funds} className={"nav-item nav-link " + changeClass}>Fondos</NavLink>
                                <NavLink to={URLs.Dashboard} className={"nav-item nav-link " + changeClass}>Dashboard</NavLink>
                                <NavLink to={URLs.FCIFBARentaPesos} className={"nav-item nav-link " + changeClass}>FCI FBA Renta Pesos</NavLink>
                                <NavLink to={URLs.PlazosFijos} className={"nav-item nav-link " + changeClass}>Plazos Fijos</NavLink>
                                <NavLink to={URLs.MercadoPago} className={"nav-item nav-link " + changeClass}>Mercado Pago</NavLink>
                                <NavLink to={URLs.Lemon} className={"nav-item nav-link " + changeClass}>Lemon</NavLink>
                                <NavLink to={URLs.Dolar} className={"nav-item nav-link " + changeClass}>Dólar</NavLink>
                                <NavLink to={URLs.IOL} className={"nav-item nav-link " + changeClass}>IOL</NavLink>
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
            </Provider>
        </div >
    );
}

export default App;
