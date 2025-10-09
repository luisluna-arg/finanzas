const fs = require("fs");
const path = require("path");

function checkUrl(name, raw, allowInsecure) {
    if (!raw) return { ok: false, msg: `${name} is missing` };
    try {
        const parsed = new URL(raw);
        if (parsed.protocol !== "https:" && !allowInsecure)
            return {
                ok: false,
                msg: `${name} uses insecure protocol ${parsed.protocol}`,
            };
        return { ok: true };
    } catch (err) {
        return { ok: false, msg: `${name} is invalid: ${err.message}` };
    }
}

function findDockerfileArgs(dir) {
    const dockerfiles = [];
    function walk(d) {
        const entries = fs.readdirSync(d, { withFileTypes: true });
        for (const e of entries) {
            const full = path.join(d, e.name);
            if (e.isDirectory()) {
                walk(full);
            } else if (e.isFile() && /Dockerfile$/i.test(e.name)) {
                dockerfiles.push(full);
            }
        }
    }
    walk(dir);
    const findings = [];
    for (const df of dockerfiles) {
        const content = fs.readFileSync(df, "utf8");
        const m = content.match(/ARG\s+([A-Za-z0-9_]+)/g);
        if (m) {
            for (const arg of m) {
                const name = arg.split(/\s+/)[1];
                if (
                    /SECRET|PASSWORD|TOKEN|CLIENT_SECRET|AUTH0_CLIENT_SECRET/i.test(
                        name
                    )
                ) {
                    findings.push({ file: df, arg: name });
                }
            }
        }
    }
    return findings;
}

module.exports = { checkUrl, findDockerfileArgs };
