import React, { useState } from 'react';

const SquadSearch = ({ onSearch }) => {
    const [teamName, setTeamName] = useState('');
    const [season, setSeason] = useState('');

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!teamName) {
            alert('Please enter a team name.');
            return;
        }
        if (season && season.length !== 4) {
            alert('Season must be exactly 4 digits.');
            return;
        }
        onSearch(teamName, season);
    };

    const handleSeasonChange = (e) => {
        const value = e.target.value;
        // Allow only 4 digits or less
        if (/^\d{0,4}$/.test(value)) {
            setSeason(value);
        }
    };

    return (
        <form onSubmit={handleSubmit} style={{ marginBottom: '20px' }}>
            <div>
                <label>
                    Team Name/Nickname:
                    <input
                        type="text"
                        value={teamName}
                        onChange={(e) => setTeamName(e.target.value)}
                        placeholder="Enter team name or nickname"
                        required
                    />
                </label>
            </div>
            <div>
                <label>
                    Season (optional):
                    <input
                        type="text" // Keep type="text" to allow validation logic
                        value={season}
                        onChange={handleSeasonChange}
                        placeholder="e.g., 2024"
                    />
                </label>
            </div>
            <button type="submit">Search</button>
        </form>
    );
};

export default SquadSearch;
