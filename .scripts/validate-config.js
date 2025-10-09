#!/usr/bin/env node
// CI config validator
// - Fails if VITE_API_URL is missing or invalid
// - Fails if VITE_API_URL uses http:// and ALLOW_INSECURE_API (or VITE_ALLOW_INSECURE_API) is not set
// - Fails if Dockerfiles contain ARG or ENV with secret-like variable names
// IMPORTANT: Do not print secret values to logs

const fs = require("fs").promises;
const path = require("path");

function isTruthy(val) {
    if (!val) return false;
    return /^(1|true|yes)$/i.test(String(val).trim());
}

(async function main() {
    try {
        const repoRoot = process.cwd();
        const viteApiUrl = process.env.VITE_API_URL;
        const allowInsecure =
            isTruthy(process.env.ALLOW_INSECURE_API) ||
            isTruthy(process.env.VITE_ALLOW_INSECURE_API);

        let failed = false;

        // Validate VITE_API_URL
        if (!viteApiUrl || !String(viteApiUrl).trim()) {
            console.error(
                "ERROR: Missing required environment variable VITE_API_URL"
            );
            failed = true;
        } else {
            // Validate URL format and scheme without echoing the full value
            try {
                const parsed = new URL(viteApiUrl);
                if (parsed.protocol === "http:" && !allowInsecure) {
                    console.error(
                        "ERROR: VITE_API_URL uses http:// but ALLOW_INSECURE_API is not set. Use https:// in production or set ALLOW_INSECURE_API for staging."
                    );
                    failed = true;
                }
            } catch (e) {
                console.error("ERROR: VITE_API_URL is not a valid URL");
                failed = true;
            }
        }

        // Scan for Dockerfiles containing secret-like ARG/ENV
        const secretPatterns = [
            "secret",
            "password",
            "pwd",
            "token",
            "client_secret",
            "connectionstring",
            "connection_string",
            "access_key",
            "secret_key",
            "api_key",
            "auth0__application__clientsecret",
            "auth0__managementapi__clientsecret",
        ];

        const matches = [];

        async function walk(dir) {
            const entries = await fs.readdir(dir, { withFileTypes: true });
            for (const e of entries) {
                const full = path.join(dir, e.name);
                if (e.isDirectory()) {
                    // skip large and irrelevant folders
                    if (
                        ["node_modules", ".git", "bin", "obj", "dist"].includes(
                            e.name
                        )
                    )
                        continue;
                    await walk(full);
                } else {
                    const lower = e.name.toLowerCase();
                    if (
                        lower === "dockerfile" ||
                        lower.endsWith("dockerfile") ||
                        lower.endsWith(".dockerfile")
                    ) {
                        let content = "";
                        try {
                            content = await fs.readFile(full, "utf8");
                        } catch (err) {
                            // ignore unreadable files
                            continue;
                        }
                        const lines = content.split(/\r?\n/);
                        for (let i = 0; i < lines.length; i++) {
                            const line = lines[i];
                            const m = line.match(/^\s*(ARG|ENV)\s+(.+)$/i);
                            if (m) {
                                const rest = m[2].toLowerCase();
                                for (const p of secretPatterns) {
                                    if (rest.includes(p)) {
                                        // record match but avoid printing the full value
                                        const varMatch =
                                            m[2].match(/([A-Za-z0-9_\-]+)/);
                                        const varName = varMatch
                                            ? varMatch[1]
                                            : "(unknown)";
                                        matches.push({
                                            file: full,
                                            line: i + 1,
                                            varName,
                                        });
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        await walk(repoRoot);

        if (matches.length > 0) {
            console.error(
                "ERROR: Found potentially secret build-time variables in Dockerfiles. Do not include secrets as ARG or ENV in Dockerfiles; provide them at runtime. Offending locations:"
            );
            for (const m of matches) {
                console.error(
                    ` - ${path.relative(repoRoot, m.file)}:${m.line} -> ${
                        m.varName
                    }`
                );
            }
            failed = true;
        }

        if (failed) {
            process.exit(1);
        }

        console.log("Validation passed");
        process.exit(0);
    } catch (err) {
        console.error(
            "Unexpected error during validation:",
            err && err.message ? err.message : String(err)
        );
        process.exit(2);
    }
})();
const path = require("path");
const { checkUrl, findDockerfileArgs } = require("./lib/validate-config-lib");

(async function main() {
    const workspace = path.resolve(__dirname, "..");
    const frontend = path.join(workspace, "FinanceFrontEnd");
    const allowInsecure =
        process.env.ALLOW_INSECURE_API === "true" ||
        process.env.VITE_ALLOW_INSECURE_API === "true";

    const checks = [];
    // Check FinanceFunds
    try {
        const ffEnv = process.env.VITE_API_URL || null;
        checks.push({
            name: "FinanceFunds VITE_API_URL",
            ...checkUrl("FinanceFunds VITE_API_URL", ffEnv, allowInsecure),
        });
    } catch (err) {
        checks.push({
            name: "FinanceFunds VITE_API_URL",
            ok: false,
            msg: String(err),
        });
    }

    // Check FinanceRemix (server-side env)
    try {
        const remixEnv =
            process.env.VITE_API_URL || process.env.VITE_API_ENDPOINT || null;
        checks.push({
            name: "FinanceRemix VITE_API_URL",
            ...checkUrl("FinanceRemix VITE_API_URL", remixEnv, allowInsecure),
        });
    } catch (err) {
        checks.push({
            name: "FinanceRemix VITE_API_URL",
            ok: false,
            msg: String(err),
        });
    }

    const dockerFindings = findDockerfileArgs(frontend);

    let failed = false;
    for (const c of checks) {
        if (!c.ok) {
            console.error(`ERROR: ${c.name}: ${c.msg}`);
            failed = true;
        } else {
            console.log(`OK: ${c.name}`);
        }
    }

    if (dockerFindings.length) {
        console.error("\nPotential secret-like Docker ARGs found:");
        for (const d of dockerFindings) {
            console.error(` - ${d.file}: ${d.arg}`);
        }
        // Now treat Dockerfile secret args as fatal to encourage remediation
        failed = true;
        console.error(
            "\nTreating Dockerfile secret ARGs as fatal. Remove secrets from Docker ARGs and inject them at runtime."
        );
    }

    if (failed) process.exit(2);
    process.exit(0);
})();
