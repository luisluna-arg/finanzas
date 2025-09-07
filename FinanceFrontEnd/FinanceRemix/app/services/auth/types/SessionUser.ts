export type SessionUser = {
    id: string;
    email: string;
    name: string;
    // Store minimal token info or reference
    tokenExpiry?: number;
};

// Convert full User to compact SessionUser
export function encodeUserForSession(user: any): SessionUser {
    return {
        id: user.id,
        email: user.email,
        name: user.name,
        tokenExpiry: user.exp ? user.exp * 1000 : undefined, // Convert to milliseconds if exists
    };
}
