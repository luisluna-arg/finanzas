export type SessionUser = {
    id: string;
    email: string;
    name: string;
    picture?: string;
    // Store minimal token info or reference
    tokenExpiry?: number;
    serverSessionId?: string;
};

// Convert full User to compact SessionUser
export function encodeUserForSession(
    user: Record<string, unknown>
): SessionUser {
    return {
        id: (user.id as string) ?? "",
        email: (user.email as string) ?? "",
        name: (user.name as string) ?? "",
        tokenExpiry: user.exp ? (user.exp as number) * 1000 : undefined, // Convert to milliseconds if exists
    };
}
