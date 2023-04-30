import {
    BrowserRouter,
    Routes,
    Route,
    NavLink
} from "react-router-dom";
import { useState, useEffect } from "react";
import { URLs } from "./../../router/urls";
import Dashboard from './../Dashboard';
import Funds from './../Funds/Funds';
import { Provider } from './../../context';
import { logoutAction, checkLoggedIn } from './../../services/AuthService';

function Home() {
    const navLinkClass = 'flex-sm-fill text-sm-center nav-link ';
    const changeClass = ({ isActive }) => navLinkClass + (isActive ? 'active' : 'nav-link inactive');

    let [loggedIn, setLoggedIn] = useState(false);

    useEffect(() => {
        setLoggedIn(checkLoggedIn());
    }, [setLoggedIn]);

    if (!loggedIn) return (<></>);

    return (
        <div className="main bg-dark">
            <Provider>
                <BrowserRouter>
                    <nav className="navbar navbar-expand-sm navbar-dark bg-success mb-3">
                        <div className="collapse navbar-collapse" id="navbar-0">
                            <div className="navbar-nav container-fluid">
                                <div className="col d-flex flex-row">
                                    <NavLink to={URLs.Funds} className={"nav-item nav-link active " + changeClass}>Fondos</NavLink>
                                    <NavLink to={URLs.Dashboard} className={"nav-item nav-link " + changeClass}>Dashboard</NavLink>
                                    <NavLink to={URLs.FCIFBARentaPesos} className={"nav-item nav-link " + changeClass}>FCI FBA Renta Pesos</NavLink>
                                    <NavLink to={URLs.PlazosFijos} className={"nav-item nav-link " + changeClass}>Plazos Fijos</NavLink>
                                    <NavLink to={URLs.MercadoPago} className={"nav-item nav-link " + changeClass}>Mercado Pago</NavLink>
                                    <NavLink to={URLs.Lemon} className={"nav-item nav-link " + changeClass}>Lemon</NavLink>
                                    <NavLink to={URLs.Dolar} className={"nav-item nav-link " + changeClass}>Dólar</NavLink>
                                    <NavLink to={URLs.IOL} className={"nav-item nav-link " + changeClass}>IOL</NavLink>
                                </div>
                                <div className="col-1 text-justify">
                                    <NavLink to="#" className={"nav-item nav-link " + changeClass + " text-end"} onClick={logoutAction}>Logout</NavLink>
                                </div>
                            </div>
                        </div>
                    </nav>
                    <div className="tab-content ps-3 pe-3">
                        <Routes>
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
                    </div>
                </BrowserRouter>
            </Provider>
        </div >
    );
}

export default Home;