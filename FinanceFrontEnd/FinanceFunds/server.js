// Minimal production server — deliberately not using `react-router-serve`,
// which hardcodes a per-request morgan access-log line with no way to turn it
// off. This replicates the same setup (compression, static assets, RR7 request
// handling) without that noise; real errors still surface via SafeLogger.error
// and React Router's own error handling.
import path from 'node:path';
import { pathToFileURL } from 'node:url';
import express from 'express';
import compression from 'compression';
import sourceMapSupport from 'source-map-support';
import { createRequestHandler } from '@react-router/express';

sourceMapSupport.install();

const BUILD_PATH = path.resolve('./build/server/index.js');
const PORT = Number(process.env.PORT) || 3000;

const build = await import(pathToFileURL(BUILD_PATH).href);
const publicPath = build.publicPath ?? '/';
const assetsBuildDirectory = build.assetsBuildDirectory ?? 'build/client';

const app = express();
app.disable('x-powered-by');
app.use(compression());
app.use(
  path.posix.join(publicPath, 'assets'),
  express.static(path.join(assetsBuildDirectory, 'assets'), { immutable: true, maxAge: '1y' })
);
app.use(publicPath, express.static(assetsBuildDirectory));

app.all('*', createRequestHandler({ build, mode: process.env.NODE_ENV }));

app.listen(PORT, () => {
  console.log(`[server] listening on port ${PORT}`);
});
