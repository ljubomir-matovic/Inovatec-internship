export interface LogsFilter {
    Message?: string | null,
    Exception?: string | null,
    User?: string | null,
    PageNumber: number,
    PageSize: number,
    SortField?: string | null,
    SortOrder: -1 | 1
}