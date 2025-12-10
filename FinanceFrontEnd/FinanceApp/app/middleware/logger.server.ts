import pino from 'pino';

// Create logger with pretty printing in development
export const logger = pino({
  level: process.env.LOG_LEVEL || 'info',
  transport: {
    target: 'pino-pretty',
    options: {
      colorize: true,
      translateTime: 'HH:MM:ss.l',
      ignore: 'pid,hostname',
      singleLine: false,
      messageFormat: '{levelLabel} - {msg}',
    }
  }
});

// Request/Response logging middleware
export function createRequestLogger() {
  return async (request: Request, responseHeaders: Headers, statusCode: number) => {
    const url = new URL(request.url);
    const cookies = request.headers.get('cookie') || 'none';
    const setCookie = responseHeaders.get('set-cookie') || 'none';
    
    logger.info({
      method: request.method,
      path: url.pathname,
      status: statusCode,
      cookies: cookies.substring(0, 100),
      setCookie: setCookie.substring(0, 100),
      userAgent: request.headers.get('user-agent')?.substring(0, 50),
    }, `${request.method} ${url.pathname} ${statusCode}`);
  };
}

// Session operation logger
export const sessionLogger = {
  sessionCheck: (sessionId: string | null, found: boolean) => {
    logger.info({ sessionId, found }, `Session check: ${sessionId ? 'ID=' + sessionId.substring(0, 8) : 'none'} found=${found}`);
  },
  
  sessionCreated: (sessionId: string) => {
    logger.info({ sessionId }, `Session created: ${sessionId.substring(0, 8)}`);
  },
  
  sessionDestroyed: (sessionId: string) => {
    logger.info({ sessionId }, `Session destroyed: ${sessionId.substring(0, 8)}`);
  },
  
  redisOperation: (operation: string, key: string, success: boolean, error?: any) => {
    if (success) {
      logger.info({ operation, key }, `Redis ${operation}: ${key}`);
    } else {
      logger.error({ operation, key, error }, `Redis ${operation} failed: ${key}`);
    }
  },
  
  authCheck: (path: string, authenticated: boolean, sessionId?: string) => {
    logger.info({ 
      path, 
      authenticated, 
      sessionId: sessionId?.substring(0, 8) 
    }, `Auth check: ${path} authenticated=${authenticated}`);
  }
};
