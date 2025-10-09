import {
    Links,
    Meta,
    Outlet,
    Scripts,
    ScrollRestoration,
} from "@remix-run/react";
import { LinksFunction, redirect, LoaderFunction } from "@remix-run/node";
import { library } from "@fortawesome/fontawesome-svg-core";
import { fas } from "@fortawesome/free-solid-svg-icons";
import Navigation from "@/components/ui/Navigation";
import stylesheet from "@/tailwind.css?url";
import app from "@/app.css?url";

library.add(fas);

// Conditional redirect based on the current path
export const loader: LoaderFunction = async ({ request }) => {
    const url = new URL(request.url);

    if (url.pathname === "/") {
        // Check if user is authenticated before redirecting to dashboard
        const { getUserFromSession } = await import(
            "@/services/auth/session.server"
        );
        const user = await getUserFromSession(request);

        if (user) {
            return redirect("/dashboard");
        } else {
            return redirect("/auth/login");
        }
    }

    return null; // No redirect, allow normal processing
};

export function Layout({ children }: { children: React.ReactNode }) {
    return (
        <html lang="en">
            <head>
                <meta charSet="utf-8" />
                <meta
                    name="viewport"
                    content="width=device-width, initial-scale=1"
                />
                <Meta />
                <Links />
            </head>
            <body>
                <Navigation />
                {children}
                <ScrollRestoration />
                <Scripts />
            </body>
        </html>
    );
}

export default function App() {
    return <Outlet />;
}

export const links: LinksFunction = () => [
    { rel: "stylesheet", href: stylesheet },
    { rel: "app", href: app },
];
