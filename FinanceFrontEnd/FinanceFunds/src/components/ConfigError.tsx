export default function ConfigError({ message }: { message: string }) {
  return (
    <div style={{ padding: 32, fontFamily: 'sans-serif' }}>
      <h1>Configuration error</h1>
      <p>{message}</p>
      <p>Please contact your administrators to fix the deployment configuration.</p>
    </div>
  );
}
