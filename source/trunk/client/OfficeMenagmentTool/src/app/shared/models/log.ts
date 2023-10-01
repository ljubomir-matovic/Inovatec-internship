export interface Log {
    id: number,
    message: string,
    exception: string,
    timeStamp: Date,
    user?: string
}