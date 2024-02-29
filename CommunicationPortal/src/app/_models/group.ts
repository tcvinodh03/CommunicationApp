

export interface Connection {
    connectionId: string;
    userName: string;
}
export interface Group {
    connectionId: string;
    connections: Connection[];
}