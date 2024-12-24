import React from 'react';

const SquadDetails = ({ squad, error, teamName, crest }) => {
    if (error) {
        return <p style={{ color: 'red', textAlign: 'center' }}>{error}</p>; // Display clean error message
    }

    if (!squad || squad.length === 0) {
        return <p style={{ textAlign: 'center' }}>No squad details available. Please search for a team.</p>;
    }

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toISOString().split('T')[0];
    };

    return (
        <div>
            <h2 style={{ textAlign: 'center', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                {crest && (
                    <img
                        src={crest}
                        alt={`${teamName} crest`}
                        style={{
                            width: '50px',
                            height: '50px',
                            marginRight: '10px',
                            borderRadius: '50%',
                        }}
                    />
                )}
                {teamName ? `${teamName} Details` : 'Squad Details'}
            </h2>
            <div className="squad-grid">
                {squad.map((player, index) => (
                    <div key={index} className="squad-grid-item">
                        <img
                            src={player.profilePicture || '/default-avatar.png'}
                            alt={`${player.firstName} ${player.surname}`}
                        />
                        <p className="bold">{player.firstName} {player.surname}</p>
                        <p><strong>Date of Birth:</strong> {formatDate(player.dateOfBirth)}</p>
                        <p><strong>Position:</strong> {player.position || 'Unknown'}</p>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default SquadDetails;
