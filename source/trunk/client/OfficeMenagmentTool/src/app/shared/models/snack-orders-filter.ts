export interface OrderFilter {
    Name?: string | null,
    Description?: string | null,
    Categories?: number[] | null,
    Users?: number[] | null,
    Items?: number[] | null,
    States?: number[] | null,
    SortField?: string | null,
    SortOrder: -1 | 1
}