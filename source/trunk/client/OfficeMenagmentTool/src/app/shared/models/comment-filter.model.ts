export interface CommentFilter {
  Users?: number[],
  Orders?: number[],
  Reports?: number[],
  GetOrderComments?: boolean,
  GetReportComments?: boolean,
  CommentsSkipCount: number,
  CommentsBatchSize: number,
  SortOrder: -1 | 1
}
