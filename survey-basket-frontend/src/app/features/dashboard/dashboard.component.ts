import { Component, inject, signal, OnInit, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatOptionModule } from '@angular/material/core';
import { RouterLink } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { PollsService } from '../../core/services/polls.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { Poll, VotesPerDay, VotesPerAnswer } from '../../core/models';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, RouterLink, BaseChartDirective,
    MatCardModule, MatIconModule, MatButtonModule,
    MatProgressSpinnerModule, MatSelectModule, MatFormFieldModule, MatOptionModule,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  private readonly pollsService = inject(PollsService);
  private readonly dashboardService = inject(DashboardService);
  private readonly authService = inject(AuthService);
  isAdminOnly = signal<boolean>(this.authService.hasRole('Admin'));

  polls = signal<Poll[]>([]);
  selectedPollId = signal<number | null>(null);
  loading = signal(true);
  chartLoading = signal(false);

  publishedCount = signal(0);
  totalPolls = signal(0);
  totalVotes = signal(0);

  // Chart data signals
  votesPerDay = signal<VotesPerDay[]>([]);
  votesPerAnswer = signal<VotesPerAnswer[]>([]);

  // Chart configurations
  lineChartConfig = computed(() => this.buildLineChartConfig());
  barChartConfig = computed(() => this.buildBarChartConfig());

  // Chart options
  readonly lineChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        display: true,
        labels: {
          usePointStyle: true,
          padding: 15,
          font: { size: 12, weight: 500 as const },
          color: 'var(--text-primary)',
        },
      },
      tooltip: {
        enabled: true,
        mode: 'index' as const,
        intersect: false,
        backgroundColor: 'rgba(0,0,0,0.8)',
        padding: 12,
        titleFont: { size: 13 },
        bodyFont: { size: 12 },
        borderColor: 'var(--accent)',
        borderWidth: 1,
      },
    },
    scales: {
      y: {
        beginAtZero: true,
        grid: { color: 'rgba(0,0,0,0.05)' },
        ticks: { color: 'var(--text-muted)', font: { size: 11 } },
      },
      x: {
        grid: { display: false },
        ticks: { color: 'var(--text-muted)', font: { size: 11 } },
      },
    },
  } as const;

  readonly barChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    indexAxis: 'y' as const,
    plugins: {
      legend: {
        display: true,
        labels: {
          usePointStyle: true,
          padding: 15,
          font: { size: 12, weight: 500 as const },
          color: 'var(--text-primary)',
        },
      },
      tooltip: {
        enabled: true,
        backgroundColor: 'rgba(0,0,0,0.8)',
        padding: 12,
        titleFont: { size: 13 },
        bodyFont: { size: 12 },
        borderColor: 'var(--accent)',
        borderWidth: 1,
        callbacks: {
          label: (context: any) => `${context.dataset.label}: ${context.parsed.x} votes`,
        },
      },
    },
    scales: {
      x: {
        beginAtZero: true,
        grid: { color: 'rgba(0,0,0,0.05)' },
        ticks: { color: 'var(--text-muted)', font: { size: 11 } },
      },
      y: {
        grid: { display: false },
        ticks: { color: 'var(--text-muted)', font: { size: 11 } },
      },
    },
  } as const;

  constructor() {
    // Watch for poll selection changes and fetch chart data
    effect(() => {
      const pollId = this.selectedPollId();
      if (pollId && this.isAdminOnly()) {
        this.loadChartData(pollId);
      }
    });
  }

  ngOnInit(): void {
    this.pollsService.getPolls().subscribe({
      next: (polls) => {
        this.polls.set(polls);
        this.totalPolls.set(polls.length);
        this.publishedCount.set(polls.filter((p) => p.isPublished).length);
        if (polls.length > 0) {
          this.selectedPollId.set(polls[0].id);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private loadChartData(pollId: number): void {
    this.chartLoading.set(true);

    this.dashboardService.getVotesPerDay(pollId).subscribe({
      next: (data) => this.votesPerDay.set(data),
      error: () => {
        this.votesPerDay.set([]);
        this.chartLoading.set(false);
      },
    });

    this.dashboardService.getVotesPerAnswer(pollId).subscribe({
      next: (data) => {
        this.votesPerAnswer.set(data);
        this.calculateTotalVotes(data);
        this.chartLoading.set(false);
      },
      error: () => {
        this.votesPerAnswer.set([]);
        this.chartLoading.set(false);
      },
    });
  }

  private calculateTotalVotes(votesPerAnswer: VotesPerAnswer[] | undefined): void {
    let total = 0;
    if (!votesPerAnswer || !Array.isArray(votesPerAnswer)) {
      this.totalVotes.set(total);
      return;
    }

    votesPerAnswer.forEach((item) => {
      if (item?.answers && Array.isArray(item.answers)) {
        item.answers.forEach((answer) => {
          total += answer?.count ?? 0;
        });
      }
    });
    this.totalVotes.set(total);
  }

  private buildLineChartConfig(): any {
    const data = this.votesPerDay();
    if (data.length === 0) return { labels: [], datasets: [] };

    return {
      labels: data.map((d) => new Date(d.date).toLocaleDateString()),
      datasets: [
        {
          label: 'Votes',
          data: data.map((d) => d.votes),
          borderColor: 'var(--primary)',
          backgroundColor: 'rgba(63, 81, 181, 0.1)',
          tension: 0.4,
          fill: true,
          pointRadius: 5,
          pointBackgroundColor: 'var(--primary)',
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
        },
      ],
    };
  }

  private buildBarChartConfig(): any {
    const data = this.votesPerAnswer();
    if (data.length === 0) return { labels: [], datasets: [] };

    const colors = ['var(--primary)', 'var(--accent)', '#f44336', '#4caf50', '#ff9800'];
    const datasets = data.map((question, idx) => ({
      label: question.questionTitle,
      data: question.answers.map((a) => a.count),
      backgroundColor: colors[idx % colors.length],
    }));

    const allAnswers = [...new Set(data.flatMap((q) => q.answers.map((a) => a.title)))];

    return {
      labels: allAnswers,
      datasets,
    };
  }
}
