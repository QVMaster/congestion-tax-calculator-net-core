# Congestion Tax Calculator

Welcome to the Persici Financial Technologies assessment.

This repository contains a developer [assignment](ASSIGNMENT.md) used as a basis for candidate intervew and evaluation.

Clone this repository to get started. Due to a number of reasons, not least privacy, you will be asked to zip your solution and share it in, instead of submitting a pull-request.



### Current status
- Core calculation logic implemented and refactored
- Single-day and multi-pass fee calculation works according to the 60-minute "single charge" rule
- Toll-free vehicles and holidays/weekends are correctly handled
- Unit tests cover key scenarios

### Next stages
1. Database integration for city-specific tax parameters
2. Support for multiple cities with different tax rules
3. Dependency injection for configuration parameters like daily cap

This staged approach ensures that each part of the system is functional, testable, and ready for future enhancements.
