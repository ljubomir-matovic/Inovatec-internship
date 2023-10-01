export interface ReportScheduleFilter {
    Offices?: number[] | null,
    PageNumber: number,
    PageSize: number,
    SortField?: string | null,
    SortOrder: -1 | 1,
    State?: number
}