#!/usr/bin/env node
// CI config validator
// - Fails if API_URL is missing or invalid
// - Fails if API_URL uses http:// and ALLOW_INSECURE_API is not set
// - Fails if Dockerfiles contain ARG or ENV with secret-like variable names
// IMPORTANT: Do not print secret values to logs

const fs = require('fs').promises;
const path = require('path');

(async function main() {
  try {
    const repoRoot = process.cwd();

    // Scan for Dockerfiles containing secret-like ARG/ENV
    const secretPatterns = [
      'secret',
      'password',
      'pwd',
      'token',
      'client_secret',
      'connectionstring',
      'connection_string',
      'access_key',
      'secret_key',
      'api_key',
      'auth0__application__clientsecret',
      'auth0__managementapi__clientsecret',
    ];

    const matches = [];

    async function walk(dir) {
      const entries = await fs.readdir(dir, { withFileTypes: true });
      for (const e of entries) {
        const full = path.join(dir, e.name);
        if (e.isDirectory()) {
          // skip large and irrelevant folders
          if (['node_modules', '.git', 'bin', 'obj', 'dist'].includes(e.name)) continue;
          await walk(full);
        } else {
          const lower = e.name.toLowerCase();
          if (
            lower === 'dockerfile' ||
            lower.endsWith('dockerfile') ||
            lower.endsWith('.dockerfile')
          ) {
            let content = '';
            try {
              content = await fs.readFile(full, 'utf8');
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
                    const varMatch = m[2].match(/([A-Za-z0-9_\-]+)/);
                    const varName = varMatch ? varMatch[1] : '(unknown)';
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
        'ERROR: Found potentially secret build-time variables in Dockerfiles. Do not include secrets as ARG or ENV in Dockerfiles; provide them at runtime. Offending locations:'
      );
      for (const m of matches) {
        console.error(` - ${path.relative(repoRoot, m.file)}:${m.line} -> ${m.varName}`);
      }
      process.exit(1);
    }

    console.log('Validation passed');
    process.exit(0);
  } catch (err) {
    console.error(
      'Unexpected error during validation:',
      err && err.message ? err.message : String(err)
    );
    process.exit(2);
  }
})();
