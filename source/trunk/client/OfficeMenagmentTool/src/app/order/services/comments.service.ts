import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppUtility } from 'src/app/shared/functions/utility';
import { ActionResultResponse } from 'src/app/shared/models/action-result-response.model';
import { CommentEdit } from 'src/app/shared/models/comment-edit.model';
import { CommentFilter } from 'src/app/shared/models/comment-filter.model';
import { Comment } from 'src/app/shared/models/comment.model';
import { DataPage } from 'src/app/shared/models/data-page.model';
import { NewComment } from 'src/app/shared/models/new-comment.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CommentsService {
  private readonly apiUrl: string = `${environment.serverUrl}/api/Comment`;

  constructor(private http: HttpClient) { }

  getComment(id: number): Observable<ActionResultResponse<Comment>> {
    return this.http.get<ActionResultResponse<Comment>>(`${this.apiUrl}/${id}`);
  }

  getComments(commentFilter: CommentFilter) {
    return this.http.get<ActionResultResponse<DataPage<Comment>>>(`${this.apiUrl}`, { params: AppUtility.getParamsFromObject(commentFilter) });
  }

  postComment(newComment: NewComment): Observable<ActionResultResponse<Comment>> {
    return this.http.post<ActionResultResponse<Comment>>(`${this.apiUrl}`, newComment);
  }

  deleteComment(id: number): Observable<ActionResultResponse<string>> {
    return this.http.delete<ActionResultResponse<string>>(`${this.apiUrl}/${id}`);
  }

  updateComment(commentEdit: CommentEdit): Observable<ActionResultResponse<string>> {
    return this.http.patch<ActionResultResponse<string>>(`${this.apiUrl}`, commentEdit);
  }
}