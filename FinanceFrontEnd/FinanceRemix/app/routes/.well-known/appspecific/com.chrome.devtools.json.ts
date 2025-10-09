import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";

/**
 * Minimal manifest for Chrome DevTools appspecific lookup.
 * This avoids the "No routes matched location" 404 when DevTools probes
 * /.well-known/appspecific/com.chrome.devtools.json
 */
export const loader: LoaderFunction = async () => {
    const payload = {
        name: "Finance - DevTools Placeholder",
        description:
            "Placeholder manifest to satisfy Chrome DevTools appspecific lookup.",
    };

    return json(payload, {
        status: 200,
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
    });
};

export const headers = () => ({
    "Cache-Control": "public, max-age=86400, immutable",
});

export const shouldRevalidate = () => false;
