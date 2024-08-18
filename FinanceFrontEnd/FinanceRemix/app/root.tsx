import {
  Links,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
} from "@remix-run/react";

import { redirect, LoaderFunction } from "@remix-run/node";

import "@/styles/index.scss";
import '@/styles/App.scss';
import Navigation from "./components/ui/Navbar/Navbar";

// Conditional redirect based on the current path
export const loader: LoaderFunction = async ({ request }) => {
  const url = new URL(request.url);

  if (url.pathname === "/") {
    return redirect('/dashboard');
  }

  return null; // No redirect, allow normal processing
};


export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
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
