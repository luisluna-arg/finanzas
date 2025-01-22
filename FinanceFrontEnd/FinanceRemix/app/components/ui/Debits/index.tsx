import { Link, Outlet, useNavigate } from "@remix-run/react";
import { useEffect } from "react";

export default function Debits() {
    const navigate = useNavigate();
  
    useEffect(() => {
      navigate("/debits/monthly");
    }, [navigate]);
  
    return (
      <div>
        <h1>Debits</h1>
        <nav>
          <Link to="/debits/monthly">Monthly</Link> |{" "}
          <Link to="/debits/annual">Annual</Link>
        </nav>
        <hr />
        <Outlet /> {/* Renders child routes like monthly or annual */}
      </div>
    );
  }
  