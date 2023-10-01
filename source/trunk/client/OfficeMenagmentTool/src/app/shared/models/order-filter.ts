export interface OrderFilter {
    Name?: string | null,
    Item?: string | null,
    Description?: string | null,
    Categories?: number[] | null,
    Offices?: number[] | null,
    Users?: number[] | null,
    Items?: number[] | null,
    States?: number[] | null,
    PageNumber: number,
    PageSize: number,
    SortField?: string | null,
    SortOrder: -1 | 1
}