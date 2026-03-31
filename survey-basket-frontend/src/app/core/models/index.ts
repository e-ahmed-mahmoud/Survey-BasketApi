// Auth
export interface AuthRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  tokenType: string;
  expiresIn: number;
  refreshToken: string;
}

export interface RefreshTokenRequest {
  token: string;
  refreshToken: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface EmailConfirmRequest {
  userId: string;
  code: string;
}

export interface ResendConfirmationRequest {
  email: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  resetCode: string;
  newPassword: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

// User Account
export interface AccountInfo {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  isEmailConfirmed: boolean;
  roles: string[];
}

export interface UpdateAccountRequest {
  firstName: string;
  lastName: string;
}

// Polls
export interface Poll {
  id: number;
  title: string;
  summary: string;
  isPublished: boolean;
  startsAt: string;
  endsAt: string;
}

export interface PollRequest {
  title: string;
  summary: string;
  isPublished: boolean;
  startsAt: string;
  endsAt: string;
}

// Questions
export interface Question {
  id: number;
  content: string;
  answers: Answer[];
}

export interface QuestionRequest {
  content: string;
  answers: AnswerRequest[];
}

export interface Answer {
  id: number;
  content: string;
}

export interface AnswerRequest {
  content: string;
}

// Votes
export interface VoteAnswers {
  questionId: number;
  answerId: number;
}

export interface VoteRequest {
  voteAnswers: VoteAnswers[];
}

export interface PollQuestion {
  id: number;
  content: string;
  answers: Answer[];
}

// Users Admin
export interface UserCreateRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  roles: string[];
}

export interface UserUpdateRequest {
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

export interface UserListItem {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  isDisabled: boolean;
  roles: string[];
}

// Roles
export interface RoleRequest {
  name: string;
  isDefault: boolean;
  permissions: string[];
}

export interface RoleItem {
  id: string;
  name: string;
  isDeleted: boolean;
  isDefault: boolean;
}

// Dashboard
export interface PollVotesSummary {
  totalVotes: number;
  questions: QuestionSummary[];
}

export interface QuestionSummary {
  question: string;
  answers: AnswerSummary[];
}

export interface AnswerSummary {
  answer: string;
  votes: number;
}

export interface VotesPerDay {
  date: string;
  votes: number;
}

export interface VotesPerAnswer {
  questionTitle: string;
  answers: { title: string; count: number }[];
}

// Pagination
export interface PaginatedList<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface PaginationFilter {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  sort?: string;
  sortDir?: 'asc' | 'desc';
}
