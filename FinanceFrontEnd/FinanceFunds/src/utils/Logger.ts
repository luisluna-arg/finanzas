/**
 * Logger utility class that handles environment-based logging
 * Only logs in development mode to keep production console clean
 */
class Logger {
  private static isDev = import.meta.env.DEV;

  /**
   * Log informational messages (only in development)
   */
  static log(message: string, ...args: any[]): void {
    if (this.isDev) {
      console.log(message, ...args);
    }
  }

  /**
   * Log warning messages (only in development)
   */
  static warn(message: string, ...args: any[]): void {
    if (this.isDev) {
      console.warn(message, ...args);
    }
  }

  /**
   * Log error messages (only in development)
   * For production errors, use Logger.error() instead
   */
  static debug(message: string, ...args: any[]): void {
    if (this.isDev) {
      console.error(message, ...args);
    }
  }

  /**
   * Log error messages (always logs, even in production)
   * Use this for actual errors that should be tracked in production
   */
  static error(message: string, ...args: any[]): void {
    console.error(message, ...args);
  }

  /**
   * Log informational messages with [INFO] prefix (only in development)
   */
  static info(message: string, ...args: any[]): void {
    if (this.isDev) {
      console.info(`[INFO] ${message}`, ...args);
    }
  }

  /**
   * Log debug messages with [DEBUG] prefix (only in development)
   */
  static debugLog(message: string, ...args: any[]): void {
    if (this.isDev) {
      console.log(`[DEBUG] ${message}`, ...args);
    }
  }

  /**
   * Log debug errors with [DEBUG] prefix (only in development)
   */
  static debugError(message: string, ...args: any[]): void {
    if (this.isDev) {
      console.error(`[DEBUG] ${message}`, ...args);
    }
  }

  /**
   * Group related log messages (only in development)
   */
  static group(label: string, callback: () => void): void {
    if (this.isDev) {
      console.group(label);
      callback();
      console.groupEnd();
    }
  }

  /**
   * Collapsed group for related log messages (only in development)
   */
  static groupCollapsed(label: string, callback: () => void): void {
    if (this.isDev) {
      console.groupCollapsed(label);
      callback();
      console.groupEnd();
    }
  }

  /**
   * Log table data (only in development)
   */
  static table(data: any): void {
    if (this.isDev) {
      console.table(data);
    }
  }

  /**
   * Start a timer (only in development)
   */
  static time(label: string): void {
    if (this.isDev) {
      console.time(label);
    }
  }

  /**
   * End a timer (only in development)
   */
  static timeEnd(label: string): void {
    if (this.isDev) {
      console.timeEnd(label);
    }
  }
}

export default Logger;
