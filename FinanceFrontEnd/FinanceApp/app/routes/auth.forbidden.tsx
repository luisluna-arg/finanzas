import type { LoaderFunction } from "react-router";
import { HttpStatusConstants } from "@/services/auth/auth.constants";

export const loader: LoaderFunction = async () => {
    return { status: HttpStatusConstants.FORBIDDEN };
};

export const meta = () => {
    return [
        {
            title: 'Acceso Denegado',
            description: 'You do not have permission to access this resource',
        },
    ];
};

export default function ForbiddenPage() {
    return (
        <div
            style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                justifyContent: "center",
                minHeight: "60vh",
            }}
        >
            <h1 style={{ color: "#d32f2f" }}>
                {HttpStatusConstants.FORBIDDEN} - Forbidden
            </h1>
            <p>You do not have permission to access this resource.</p>
            <a
                href="/auth/login"
                style={{ color: "#1976d2", textDecoration: "underline" }}
            >
                Return to Login
            </a>
        </div>
    );
}
