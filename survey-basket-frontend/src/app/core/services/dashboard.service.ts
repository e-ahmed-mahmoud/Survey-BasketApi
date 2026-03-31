import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PollVotesSummary, VotesPerDay, VotesPerAnswer } from '../models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly base = (pollId: number) =>
    `${environment.apiUrl}/api/Polls/${pollId}/Dashboard`;

  getPollVotes(pollId: number): Observable<PollVotesSummary> {
    return this.http.get<PollVotesSummary>(`${this.base(pollId)}/GetPollVotes`);
  }

  getVotesPerDay(pollId: number): Observable<VotesPerDay[]> {
    return this.http.get<VotesPerDay[]>(`${this.base(pollId)}/GetPollVotesPerDay`);
  }

  getVotesPerAnswer(pollId: number): Observable<VotesPerAnswer[]> {
    return this.http.get<VotesPerAnswer[]>(`${this.base(pollId)}/GetPollVotesPerAnswer`);
  }
}
