import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { HttpStatusConstants } from "@/services/auth/auth.constants";

export const loader: LoaderFunction = async () => {
    return { status: HttpStatusConstants.FORBIDDEN };
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
