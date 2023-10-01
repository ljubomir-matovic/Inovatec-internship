export interface NewComment {
    userId: number,
    orderId?: number,
    reportId?: number,
    text: string
}